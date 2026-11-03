using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH112073_MyStore.Models;
using _24DH112073_MyStore.Models.ViewModel;

namespace _24DH112073_MyStore.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Home
        public ActionResult Index()
        {
            // 1. TRUY VẤN DỮ LIỆU THỐNG KÊ
            var stats = db.Products
                          .GroupBy(p => p.Category.CategoryName) // Nhóm theo Tên Danh mục
                          .Select(g => new CategoryStatVM
                          {
                              CategoryName = g.Key,
                              ProductCount = g.Count(),
                              MinPrice = g.Min(p => p.ProductPrice),
                              MaxPrice = g.Max(p => p.ProductPrice),
                              AvgPrice = g.Average(p => p.ProductPrice)
                          }).ToList();

            // 2. CHUẨN BỊ DỮ LIỆU CHO BIỂU ĐỒ (Google Chart)
            // Chuyển dữ liệu thống kê sang dạng JSON để JavaScript có thể đọc
            var chartData = new List<object>();
            chartData.Add(new object[] { "Loại hàng", "Số lượng" }); // Thêm tiêu đề

            foreach (var item in stats)
            {
                chartData.Add(new object[] { item.CategoryName, item.ProductCount });
            }

            // Lưu chuỗi JSON vào ViewBag
            ViewBag.ChartData = new HtmlString(Newtonsoft.Json.JsonConvert.SerializeObject(chartData));


            // 3. Trả về View với model là danh sách thống kê
            return View(stats);
        }
    }
}