using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _24DH112073_MyStore.Models;
using PagedList; // Thư viện phân trang
using PagedList.Mvc; // Thư viện phân trang
using _24DH112073_MyStore.Models.ViewModel; // Namespace của ViewModel bạn vừa tạo

namespace _24DH112073_MyStore.Areas.Admin.Controllers
{
    public class ProductsController : BaseController
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Products (ĐÃ CẬP NHẬT)
        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            // 1. Khởi tạo ViewModel
            var model = new ProductSearchVM();

            // --- SỬA DÒNG NÀY ---
            // Thêm .Where(p => p.IsActive == true) để chỉ lấy sản phẩm ĐANG HOẠT ĐỘNG
            var products = db.Products
                             .Include(p => p.Category)
                             .Where(p => p.IsActive == true) // <--- QUAN TRỌNG NHẤT
                             .AsQueryable();
            // --------------------

            // 2. Logic Tìm kiếm (Giữ nguyên)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDescription.Contains(searchTerm) ||
                    p.Category.CategoryName.Contains(searchTerm));
            }

            // Tìm kiếm giá (Giữ nguyên)
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.ProductPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.ProductPrice <= maxPrice.Value);
            }

            // 3. Logic Sắp xếp (Giữ nguyên)
            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }

            // Gán lại giá trị cho View (Giữ nguyên)
            model.SearchTerm = searchTerm;
            model.MinPrice = minPrice;
            model.MaxPrice = maxPrice;
            model.SortOrder = sortOrder;

            // 4. Phân trang (Giữ nguyên)
            int pageNumber = (page ?? 1);
            int pageSize = model.PageSize;

            model.Products = products.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,Quantity,Color,ProductSize")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.IsActive = true;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,Quantity,Color,ProductSize")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // 1. Tìm sản phẩm
            Product product = db.Products.Find(id);

            // --- SỬA TỪ ĐÂY ---

            // Code CŨ (Gây lỗi): db.Products.Remove(product);
            // Bạn hãy XÓA dòng trên hoặc comment nó lại

            // Code MỚI (Xóa mềm):
            product.IsActive = false; // Đánh dấu là "Đã xóa"

            // Báo cho Database biết là mình đang Sửa dòng này
            db.Entry(product).State = EntityState.Modified;

            // --- ĐẾN ĐÂY ---

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        // 1. Action hiển thị danh sách sản phẩm đã xóa (Thùng rác)
        public ActionResult Trash()
        {
            // Lấy các sản phẩm có IsActive = false (Đã xóa)
            var deletedProducts = db.Products.Where(p => p.IsActive == false).ToList();
            return View(deletedProducts);
        }

        // 2. Action khôi phục sản phẩm
        public ActionResult Undo(int id)
        {
            var product = db.Products.Find(id);

            // Đổi trạng thái lại thành True (Đang bán)
            product.IsActive = true;

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            // Quay lại trang thùng rác
            return RedirectToAction("Trash");
        }

        // 1. GET: Hiển thị trang xác nhận xóa vĩnh viễn
        public ActionResult HardDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // 2. POST: Thực hiện xóa vĩnh viễn
        [HttpPost, ActionName("HardDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult HardDeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            try
            {
                db.Products.Remove(product); // Lệnh xóa thật sự khỏi DB
                db.SaveChanges();
                return RedirectToAction("Trash");
            }
            catch (Exception ex)
            {
                // Nếu có lỗi (ví dụ: sản phẩm đã có đơn hàng), giữ lại trang và báo lỗi
                ViewBag.Error = "Không thể xóa vĩnh viễn sản phẩm này vì đã có dữ liệu liên quan (đơn hàng, chi tiết...).";
                return View(product);
            }
        }
    }
}
