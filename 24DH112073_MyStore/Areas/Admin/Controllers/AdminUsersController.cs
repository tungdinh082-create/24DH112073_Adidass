using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _24DH112073_MyStore.Models;
using PagedList;
using _24DH112073_MyStore.Models.ViewModel; 

namespace _24DH112073_MyStore.Areas.Admin.Controllers
{
    public class AdminUsersController : BaseController
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/AdminUsers
        public ActionResult Index(int? page)
        {
            var model = new AdminUserSearchVM();

            // LỌC TÀI KHOẢN:
            // Giả sử Model "User" của bạn có cột "Role" (vai trò)
            // và chỉ tài khoản có Role = "Admin" mới là Admin.
            // HÃY SỬA LẠI "Role" VÀ "Admin" CHO ĐÚNG VỚI DATABASE CỦA BẠN
            var users = db.Users
                          .Where(u => u.UserRole == "A") // <-- Lọc chỉ Admin
                          .OrderBy(u => u.Username);

            // Phân trang
            int pageNumber = (page ?? 1);
            int pageSize = model.PageSize;
            model.AdminUsers = users.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Admin/AdminUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/AdminUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,UserRole")] User user)
        {
            try // <-- BẮT ĐẦU TRY
            {
                if (ModelState.IsValid)
                {
                    user.UserRole = "A"; // Gán vai trò
                    db.Users.Add(user);
                    db.SaveChanges(); // Sẽ thử lưu
                    return RedirectToAction("Index");
                }
            }
            // CATCH LỖI VALIDATION
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Lấy thông báo lỗi chi tiết
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);

                // Gửi lỗi này ra View để bạn có thể đọc
                ViewBag.Error = "LỖI VALIDATION: " + fullErrorMessage;

                return View(user); // Quay lại trang Create và hiển thị lỗi
            }
            // CATCH LỖI TRÙNG KEY (TRÙNG TÊN ĐĂNG NHẬP)
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ViewBag.Error = "LỖI DATABASE: Tên đăng nhập này đã tồn tại.";
                return View(user);
            }
            // CATCH CÁC LỖI KHÁC
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi không xác định: " + ex.Message;
                return View(user);
            }

            return View(user);
        }

        // GET: Admin/AdminUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/AdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Password,UserRole")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Admin/AdminUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
