using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH112073_MyStore.Models;
using _24DH112073_MyStore.Models.ViewModel;
using System.Web.Security;
using System.Data.Entity;

namespace _24DH112073_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        private const string CustomerSession = "CustomerSession";

        // GET: Account/Register
        public ActionResult Register()
        {
            return View(new RegisterVM()); // Trả về View với model RegisterVM rỗng
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra xem Username đã tồn tại chưa
                var existingUser = db.Users.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model);
                }

                try
                {
                    // 2. Tạo tài khoản (User)
                    User user = new User();
                    user.Username = model.Username;

                    // --- SỬA DÒNG NÀY (QUAN TRỌNG NHẤT) ---
                    // Phải mã hóa MD5 trước khi lưu để khớp với hàm Login/ChangePassword
                    user.Password = GetMD5(model.Password);
                    // --------------------------------------

                    user.UserRole = "C";
                    db.Users.Add(user);
                    db.SaveChanges();

                    // 3. Tạo thông tin khách hàng (Customer)
                    Customer customer = new Customer();
                    customer.Username = user.Username;
                    customer.CustomerName = model.CustomerName;
                    customer.CustomerPhone = model.CustomerPhone;
                    customer.CustomerEmail = model.CustomerEmail;
                    customer.CustomerAddress = model.CustomerAddress;
                    db.Customers.Add(customer);
                    db.SaveChanges();

                    // 4. Đăng nhập cho khách hàng luôn
                    // Sửa lỗi thiếu dấu ngoặc kép ở Session
                    Session["CustomerSession"] = customer;
                    FormsAuthentication.SetAuthCookie(model.Username, false);

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }

            return View(model);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View(new LoginVM()); 
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Username == model.Username);
                if (user != null)
                {
                    // 1. Lấy dữ liệu để soi
                    string dbPass = user.Password.Trim();
                    string inputPassHash = GetMD5(model.Password);
                    string inputPlain = model.Password;

                    // 2. Kiểm tra
                    if (dbPass == inputPassHash || dbPass == inputPlain)
                    {
                        //MẬT KHẨU ĐÚNG

                        
                        var customer = db.Customers.FirstOrDefault(c => c.Username == user.Username);
                        Session["CustomerSession"] = customer;
                        System.Web.Security.FormsAuthentication.SetAuthCookie(model.Username, false);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {

                        //MẬT KHẨU SAI

                        string debugMsg = $"Lỗi Đăng nhập! DB đang lưu: '{dbPass}'. Bạn nhập mã hóa: '{inputPassHash}'. Bạn nhập thường: '{inputPlain}'";

                        ModelState.AddModelError("", debugMsg);
                    }
                }
                else
                {
                    ModelState.AddModelError("", " SAI TÀI KHOẢN HOẶC MẬT KHẨU .");
                }
            }

            
            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session[CustomerSession] = null; 
            FormsAuthentication.SignOut(); 
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/PurchaseHistory
        public ActionResult PurchaseHistory()
        {
            var customer = Session["CustomerSession"] as Customer;
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = db.Orders
                           .Where(o => o.CustomerID == customer.CustomerID)
                           .OrderByDescending(o => o.OrderDate)
                           .ToList();

            return View(orders);
        }
        // GET: Account/ProfileInfo
        [Authorize] // Bắt buộc phải đăng nhập
        public ActionResult ProfileInfo()
        {
            // 1. Kiểm tra xem khách hàng đã đăng nhập chưa 
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Lấy thông tin Customer từ Username
            var customer = db.Customers.FirstOrDefault(c => c.Username == User.Identity.Name);
            if (customer == null)
            {
                
                return RedirectToAction("Index", "Home");
            }

            // 3. Gửi thông tin Customer sang View
            return View(customer);
        }
        // GET: Account/UpdateProfile
        [Authorize] // Bắt buộc phải đăng nhập
        public ActionResult UpdateProfile()
        {
            // 1. Lấy thông tin Customer đang đăng nhập
            var customer = db.Customers.FirstOrDefault(c => c.Username == User.Identity.Name);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Tạo ViewModel từ thông tin Customer
            var model = new UpdateProfileVM
            {
                CustomerName = customer.CustomerName,
                CustomerPhone = customer.CustomerPhone,
                CustomerEmail = customer.CustomerEmail,
                CustomerAddress = customer.CustomerAddress
            };

            // 3. Gửi ViewModel sang View
            return View(model);
        }

        // POST: Account/UpdateProfile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(UpdateProfileVM model)
        {
            if (ModelState.IsValid) 
            {
                try
                {
                    // 1. Lấy Customer từ DB
                    var customer = db.Customers.FirstOrDefault(c => c.Username == User.Identity.Name);
                    if (customer == null)
                    {
                        return HttpNotFound();
                    }

                    // 2. Cập nhật thông tin từ ViewModel
                    customer.CustomerName = model.CustomerName;
                    customer.CustomerPhone = model.CustomerPhone;
                    customer.CustomerEmail = model.CustomerEmail;
                    customer.CustomerAddress = model.CustomerAddress;

                    // 3. Lưu thay đổi vào DB
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();

                    // 4. Cập nhật lại Session (Quan trọng!)
                    Session["CustomerSession"] = customer;

                    // 5. Chuyển hướng về trang Thông tin tài khoản
                    return RedirectToAction("ProfileInfo");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }

            // Nếu có lỗi, hiển thị lại form
            return View(model);
        }
        // GET: Account/ChangePassword
        [Authorize] // Bắt buộc phải đăng nhập
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordVM());
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // 1. Mã hóa mật khẩu cũ người dùng nhập
                string inputOldHash = GetMD5(model.OldPassword);

                // 2. Lấy mật khẩu trong DB và CẮT BỎ KHOẢNG TRẮNG (Quan trọng)
                string dbPassword = user.Password.Trim();

                // 3. So sánh
                // Nếu KHÔNG khớp mã hóa VÀ KHÔNG khớp chữ thường
                if (dbPassword != inputOldHash && dbPassword != model.OldPassword)
                {
                    ModelState.AddModelError("OldPassword", "Mật khẩu cũ không chính xác.");
                    return View(model);
                }

                // 4. Cập nhật mật khẩu mới (Mã hóa trước khi lưu)
                user.Password = GetMD5(model.NewPassword);

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("ProfileInfo");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
            }

            return View(model);
        }
        // Hàm mã hóa MK
        public static string GetMD5(string str)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
    }
}