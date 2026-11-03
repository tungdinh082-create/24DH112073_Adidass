using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _24DH112073_MyStore.Models;

namespace _24DH112073_MyStore.Areas.Admin.Controllers
{
    public class CategoriesController : BaseController
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Categories
        //lấy dữ liệu từ bảng categories từ DB để hiển thị
        // GET: Admin/Categories
        // SỬA LẠI HÀM NÀY
        public ActionResult Index()
        {
            // Thêm .Include(c => c.Products) để lấy danh sách sản phẩm liên quan
            var categories = db.Categories.Include(c => c.Products).ToList();
            return View(categories);
        }

        // GET: Admin/Categories/Details/5
        // dentails: lấy chi tiết 1 bản ghi có categorisID = id;
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // SỬA: Dùng Include để tải cả danh sách Products (sản phẩm) liên quan
            Category category = db.Categories
                                  .Include(c => c.Products)
                                  .FirstOrDefault(c => c.CategoryID == id);

            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category); // Gửi cả Category và Products sang View
        }

        // GET: Admin/Categories/Create
        //Load form Creat
        //[HttpGet] là phương thức mặc định nên không cần khai báo
        public ActionResult Create()
        {
            return View();
        }

       


        // GET: Admin/Categories/Edit/5\
        //Get:lấy dữ liệu của 1 danh mục đã có sao cho categoryId = id
        public ActionResult Edit(int? id)
        {
            return Details(id);
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Category category = db.Categories.Find(id);
            //if (category == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                string newName = category.CategoryName.Trim().ToUpper();
                bool isExist = db.Categories.Any(c =>
                    c.CategoryName.Trim().ToUpper() == newName &&
                    c.CategoryID != category.CategoryID
                );

                if (isExist)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục này đã tồn tại!");
                    return View(category);
                }

                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                string newName = category.CategoryName.Trim().ToUpper();
                bool isExist = db.Categories.Any(c => c.CategoryName.Trim().ToUpper() == newName);

                if (isExist)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục này đã tồn tại!");
                    return View(category);
                }

                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            return Details(id);
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Category category = db.Categories.Find(id);
            //if (category == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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
    }
}
