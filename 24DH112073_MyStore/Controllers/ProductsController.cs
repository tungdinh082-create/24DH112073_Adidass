using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH112073_MyStore.Models;
using System.Data.Entity;

namespace _24DH112073_MyStore.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Products
        public ActionResult Index()
        {
            
            return View();
        }

        // DÙNG ĐỂ TẠO MENU DANH MỤC
        
        [ChildActionOnly] 
        public ActionResult CategoryMenu()
        {
            // Lấy tất cả danh mục
            var categories = db.Categories.ToList();

            
            return PartialView("_CategoryMenu", categories);
        }
        // GET: Products/ProductsByCategory/5
        public ActionResult ProductsByCategory(int categoryId)
        {
            var products = db.Products
                     .Include(p => p.Category) 
                     .Include(p => p.OrderDetails) 
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
        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

       
            return View(product);
        }
    }
}