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
    public class CustomersController : BaseController
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Customers 
        public ActionResult Index(string searchTerm, string sortOrder, int? page)
        {
            // 1. Khởi tạo ViewModel và truy vấn
            var model = new CustomerSearchVM();
            // Giữ lại .Include(c => c.User) từ code gốc của bạn
            var customers = db.Customers.Include(c => c.User).AsQueryable();

            // 2. Logic Tìm kiếm (theo Tên hoặc Email)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                customers = customers.Where(c =>
                    c.CustomerName.Contains(searchTerm) ||
                    c.CustomerEmail.Contains(searchTerm));
            }
            // 3. Logic Sắp xếp (theo Tên)
            switch (sortOrder)
            {
                case "name_desc":
                    customers = customers.OrderByDescending(c => c.CustomerName);
                    break;
                default: // Mặc định "name_asc"
                    customers = customers.OrderBy(c => c.CustomerName);
                    break;
            }

            // Gán lại giá trị tìm kiếm/sắp xếp để giữ trạng thái trên View
            model.SearchTerm = searchTerm;
            model.SortOrder = sortOrder;

            // 4. Logic Phân trang
            int pageNumber = (page ?? 1);
            int pageSize = model.PageSize; // Lấy từ ViewModel
            model.Customers = customers.ToPagedList(pageNumber, pageSize);

            return View(model);
        }
        // GET: Admin/Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            // NÂNG CẤP: Lấy danh sách đơn hàng của khách hàng này
            // (Giả sử Model Customer có Navigation Property tên là 'Orders')
            var orders = db.Orders.Where(o => o.CustomerID == id.Value).ToList();
            ViewBag.Orders = orders; // Gửi danh sách đơn hàng sang View

            return View(customer);
        }

        // GET: Admin/Customers/Create
        public ActionResult Create()
        {
            ViewBag.Username = new SelectList(db.Users, "Username", "Password");
            return View();
        }

        // POST: Admin/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Username = new SelectList(db.Users, "Username", "Password", customer.Username);
            return View(customer);
        }

        // GET: Admin/Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.Username = new SelectList(db.Users, "Username", "Password", customer.Username);
            return View(customer);
        }
        // POST: Admin/Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Username = new SelectList(db.Users, "Username", "Password", customer.Username);
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // 1. Tìm Khách hàng
                Customer customer = db.Customers.Find(id);
                if (customer == null)
                {
                    return HttpNotFound();
                }

                // 2. Tìm tất cả Đơn hàng của khách hàng này
                var orders = db.Orders.Where(o => o.CustomerID == id).ToList();

                if (orders.Any())
                {
                    // 3. Xóa tất cả Chi tiết Đơn hàng (OrderDetails) của các đơn hàng đó
                    foreach (var order in orders)
                    {
                        var orderDetails = db.OrderDetails.Where(od => od.OrderID == order.OrderID).ToList();
                        db.OrderDetails.RemoveRange(orderDetails);
                    }

                    // 4. Xóa tất cả Đơn hàng (Orders)
                    db.Orders.RemoveRange(orders);
                }

                // 5. Xóa Khách hàng
                // LƯU Ý: Code này KHÔNG xóa User (tài khoản đăng nhập). 
                // Nó chỉ xóa thông tin Khách hàng (tên, địa chỉ...).
                db.Customers.Remove(customer);

                // 6. Lưu tất cả thay đổi
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, quay lại trang Delete và báo lỗi
                TempData["ErrorMessage"] = "Xóa thất bại: " + ex.Message;
                return RedirectToAction("Delete", new { id = id });
            }
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
