using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH112073_MyStore.Models;

namespace _24DH112073_MyStore.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Auth/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Admin/Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                // Giả sử Model của bạn là "User" và có "Username", "Password"
                // THAY THẾ "User" BẰNG TÊN MODEL TÀI KHOẢN ADMIN CỦA BẠN (ví dụ: AdminUser)
                var adminAccount = db.Users
                      .FirstOrDefault(u => u.Username == username && u.Password == password && u.UserRole == "A");

                if (adminAccount != null)
                {
                    // Đăng nhập thành công
                    // Lưu thông tin admin vào Session
                    Session["AdminUser"] = adminAccount;

                    // Chuyển hướng đến trang chủ Admin (Trang thống kê)
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Đăng nhập thất bại
                    ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                    return View();
                }
            }
            return View();
        }

        // GET: Admin/Auth/Logout
        public ActionResult Logout()
        {
            // Xóa Session
            Session["AdminUser"] = null;

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Login", "Auth");
        }
    }
}