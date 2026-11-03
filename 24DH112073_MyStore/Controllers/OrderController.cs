using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH112073_MyStore.Models;
using _24DH112073_MyStore.Models.ViewModel;
using System.Net;
using PagedList;
using System.Data.Entity;


namespace _24DH112073_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        private CartService GetCartService()
        {
            return new CartService(this.Session);
        }

        // GET: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            var cart = GetCartService().GetCart();
            if (cart.Items == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);

            var model = new CheckoutVM
            {
                CartItems = cart.Items.ToList(),
                TotalAmount = cart.TotalValue(),
                OrderDate = DateTime.Now,
                AddressDelivery = customer.CustomerAddress, // <-- SỬA 3: Lấy từ CustomerAddress
                CustomerID = customer.CustomerID,
                Username = customer.Username
            };

            return View(model);
        }

        // POST: Order/Checkout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            var cart = GetCartService().GetCart();

            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);

                if (model.PaymentMethod == "PayPal")
                {
                    return RedirectToAction("PaymentWithPaypal", "Paypal", model);
                }

                Order order = new Order();
                order.CustomerID = customer.CustomerID;
                order.OrderDate = DateTime.Now;
                order.TotalAmount = cart.TotalValue();
                order.PaymentStatus = model.PaymentStatus ?? "Chưa thanh toán";
                order.PaymentMethod = model.PaymentMethod;
                order.DeliveryMethod = model.DeliveryMethod; 
                order.AddressDelivery = model.AddressDelivery; 
                db.Orders.Add(order);
                db.SaveChanges();

                foreach (var item in cart.Items)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    db.OrderDetails.Add(orderDetail);

                    // 2. Trừ số lượng tồn kho
                    var product = db.Products.Find(item.ProductID);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity; // Trừ số lượng tồn
                        db.Entry(product).State = EntityState.Modified; 
                    }
                }
                db.SaveChanges();

                GetCartService().ClearCart();

                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }

            model.CartItems = cart.Items.ToList();
            return View(model);
        }

        // GET: Order/OrderSuccess
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders.Include("OrderDetails.Product")
                           .SingleOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Order/MyOrder (Lịch sử mua hàng)
        [Authorize]
        public ActionResult MyOrder(string searchTerm) 
        {
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.SearchTerm = searchTerm; 

            //  3: Thêm .Include() để có thể tìm theo Tên Sản phẩm
            var orders = db.Orders
                           .Include(o => o.OrderDetails.Select(od => od.Product))
                           .Where(o => o.CustomerID == customer.CustomerID);

            //  4: Thêm logic tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                int searchId = 0;
                bool isSearchId = int.TryParse(searchTerm, out searchId); 
                orders = orders.Where(o =>
                    // 1. Tìm theo Tên Sản phẩm
                    o.OrderDetails.Any(od => od.Product.ProductName.Contains(searchTerm)) ||
                    // 2. Tìm theo Mã Đơn hàng (nếu là số)
                    (isSearchId && o.OrderID == searchId)
                );
            }

            return View(orders.OrderByDescending(o => o.OrderDate).ToList());
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