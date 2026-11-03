using _24DH112073_MyStore.Models;
using _24DH112073_MyStore.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace _24DH112073_MyStore.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Home/Index 
        public ActionResult Index(string SearchTerm, int? page)
        {
            // 1. Khởi tạo ViewModel
            var model = new HomeProductVM();
            model.SearchTerm = SearchTerm;

            // 2. Bắt đầu truy vấn (lấy tất cả sản phẩm)            
            // Điều này sẽ giúp TẤT CẢ các danh sách bên dưới (Sản phẩm mới, Bán chạy, Tìm kiếm)
            // đều tự động lọc bỏ hàng đã xóa.
            var products = db.Products
                             .Include(p => p.Category)
                             .Where(p => p.IsActive == true) // thùng rác
                             .AsQueryable();

            // 3. Logic Tìm kiếm (nếu có)
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                products = products.Where(p => p.ProductName.Contains(SearchTerm) ||
                                               p.ProductDescription.Contains(SearchTerm) ||
                                               p.Category.CategoryName.Contains(SearchTerm));
            }

            // 4. Logic Lấy Sản phẩm nổi bật (bán chạy nhất)
           
            model.FeaturedProducts = products
                                     .OrderByDescending(p => p.OrderDetails.Count())
                                     .Take(10)
                                     .ToList();

            // 5. Logic Lấy Sản phẩm mới (ít bán) và Phân trang
            int pageNumber = (page ?? 1);
            int pageSize = model.PageSize;

            model.NewProducts = products
                                .OrderBy(p => p.OrderDetails.Count())
                                .Take(20)
                                .ToPagedList(pageNumber, pageSize);

            // 6. Logic "Bạn có thể thích" (Lấy ngẫu nhiên)
            
            model.YouMightLikeProducts = db.Products
                                           .Where(x => x.IsActive == true) 
                                           .OrderBy(x => Guid.NewGuid())   
                                           .Take(6)
                                           .ToList();

            return View(model); // Trả về View với ViewModel
        }
        // GET: Home/ProductDetail/5
        public ActionResult ProductDetail(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // 1. Tìm sản phẩm chính (SỬA QUAN TRỌNG)
           
            Product pro = db.Products.FirstOrDefault(x => x.ProductID == id && x.IsActive == true);

            // Nếu không tìm thấy hoặc sản phẩm đã bị "Xóa mềm" -> Báo lỗi
            if (pro == null)
            {
                return HttpNotFound();
            }

            // 2. Khởi tạo ViewModel
            var model = new ProductDetailsVM();
            model.Product = pro;

            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }
            else
            {
                model.quantity = 1; 
            }

            // 3. Lấy Sản phẩm cùng danh mục (RelatedProducts)
            
            var relatedProducts = db.Products
                                    .Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID && p.IsActive == true)
                                    .OrderBy(p => p.ProductID) 
                                    .ToPagedList(1, 8); // Trang 1, lấy 8 cái

            model.RelatedProducts = relatedProducts;

            // 4. Lấy Sản phẩm bán chạy (TopProducts)
           
            int topPageNumber = (page ?? 1);
            int topPageSize = model.PageSize; 

            model.TopProducts = db.Products
                                  .Where(p => p.IsActive == true) 
                                  .OrderByDescending(p => p.OrderDetails.Count()) 
                                  .Take(8)
                                  .ToPagedList(topPageNumber, topPageSize);

            return View(model); // Trả về View với ViewModel
        }
        // Action này dùng cho link danh mục trên menu
        public ActionResult ProductsByCategory(int categoryId)
        {
            var products = db.Products
                             .Where(p => p.CategoryID == categoryId)
                             .ToList();

            var category = db.Categories.Find(categoryId);
            if (category != null)
            {
                ViewBag.CategoryName = category.CategoryName;
            }
            else
            {
                ViewBag.CategoryName = "Không tìm thấy danh mục";
            }
            return View(products);
        }

    }
}