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
    public class OrdersController : BaseController
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Orders
        // GET: Admin/Orders (ĐÃ NÂNG CẤP)
        public ActionResult Index(string searchTerm, string status, int? page)
        {
            var model = new OrderSearchVM();

            // Dùng .Include() để lấy thông tin Khách hàng (Customer)
            var orders = db.Orders.Include(o => o.Customer).AsQueryable();

            // Logic Lọc theo trạng thái (PaymentStatus)
            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.PaymentStatus == status);
            }
            // Gửi danh sách các trạng thái (đã lọc trùng) sang View để làm dropdown
            ViewBag.StatusList = new SelectList(db.Orders.Select(o => o.PaymentStatus).Distinct().ToList());

            // Logic Tìm kiếm (theo Tên Khách hàng)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                orders = orders.Where(o => o.Customer.CustomerName.Contains(searchTerm));
            }

            // Sắp xếp (đơn hàng mới nhất lên đầu)
            orders = orders.OrderByDescending(o => o.OrderDate);

            // Gán lại giá trị cho model
            model.SearchTerm = searchTerm;
            model.Status = status;

            // Phân trang
            int pageNumber = (page ?? 1);
            int pageSize = model.PageSize;
            model.Orders = orders.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Admin/Orders/Details/5
        // GET: Admin/Orders/Details/5 (ĐÃ NÂNG CẤP)
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Include Customer để biết ai đặt
            Order order = db.Orders.Include(o => o.Customer).FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // LẤY DANH SÁCH CHI TIẾT SẢN PHẨM CỦA ĐƠN HÀNG
            // Dùng .Include(od => od.Product) để lấy cả thông tin Sản phẩm
            var orderDetails = db.OrderDetails.Include(od => od.Product)
                                             .Where(od => od.OrderID == id.Value)
                                             .ToList();

            ViewBag.OrderDetails = orderDetails; // Gửi sang View

            return View(order);
        }

        // GET: Admin/Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            return View();
        }

        // POST: Admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,CustomerID,OrderDate,TotalAmount,PaymentStatus,AddressDelivery")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", order.CustomerID);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", order.CustomerID);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,CustomerID,OrderDate,TotalAmount,PaymentStatus,AddressDelivery")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", order.CustomerID);
            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
