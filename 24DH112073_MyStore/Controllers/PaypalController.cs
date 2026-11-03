using _24DH112073_MyStore.Models; 
using _24DH112073_MyStore.Models.ViewModel;
using PayPal.Api; 
using System;
using System.Collections.Generic;
using System.Configuration; 
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Item = PayPal.Api.Item;
using Payer = PayPal.Api.Payer;

namespace _24DH112073_MyStore.Controllers
{
    public class PaypalController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // HÀM LẤY DỊCH VỤ GIỎ HÀNG (MỚI)
        private CartService GetCartService()
        {
            return new CartService(this.Session);
        }

        private APIContext GetAPIContext()
        {
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            return new APIContext(accessToken);
        }

        // HÀM NÀY BẮT ĐẦU QUÁ TRÌNH THANH TOÁN
        public ActionResult PaymentWithPaypal(CheckoutVM model)
        {
            // 1. Ép dùng giao thức bảo mật TLS 1.2 
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            try
            {
                // 2. CẤU HÌNH BẰNG DICTIONARY 
                var config = new Dictionary<string, string>();

                // Thêm chế độ sandbox
                config.Add("mode", "sandbox");

                // Thêm Client ID (Mã mới bạn vừa tạo)
                config.Add("clientId", "AUYJKjiga67n8FlvzV15iimges9rQ4VNKU6VFxTEl68lxundAPwaRWCuD3vn7f-MIEK8wDfr0iUSzpKy");

                // Thêm Secret Key (Mã mới bạn vừa tạo)
                config.Add("clientSecret", "ENEY74YjB8ixxOaoSj_DqNY8T2JtwAwwVBN72KT2E3rjdbMN8AoJhGEa9lqrMmkl6LIrqA8Z_IjfOxcs");

                // 3. Lấy Token (Chỉ truyền config vào, không truyền lẻ tẻ)
                string accessToken = new OAuthTokenCredential(config).GetAccessToken();

                // 4. Tạo APIContext
                APIContext apiContext = new APIContext(accessToken);
                apiContext.Config = config;

                // -----------------------------------------------------------
                // PHẦN DƯỚI NÀY GIỮ NGUYÊN LOGIC CŨ CỦA BẠN
                // -----------------------------------------------------------

                string guid = Guid.NewGuid().ToString();
                var itemList = new ItemList() { items = new List<Item>() };

                // Lấy giỏ hàng
                var cart = GetCartService().GetCart();
                if (cart.Items == null || !cart.Items.Any())
                {
                    return RedirectToAction("Index", "Cart");
                }

                // Tính tiền (VND -> USD)
                double total_usd = 0;
                foreach (var cartItem in cart.Items)
                {
                    double price_usd = Math.Round((double)cartItem.UnitPrice / 24000, 2);
                    itemList.items.Add(new Item()
                    {
                        name = cartItem.ProductName,
                        currency = "USD",
                        price = price_usd.ToString("0.00"),
                        quantity = cartItem.Quantity.ToString(),
                        sku = cartItem.ProductID.ToString()
                    });
                    total_usd += price_usd * cartItem.Quantity;
                }

                var transaction = new Transaction()
                {
                    amount = new Amount()
                    {
                        currency = "USD",
                        total = total_usd.ToString("0.00"),
                        details = new Details() { subtotal = total_usd.ToString("0.00"), tax = "0", shipping = "0" }
                    },
                    item_list = itemList,
                    description = "Order from MyStore",
                    invoice_number = guid
                };

                var payer = new Payer() { payment_method = "paypal" };

                var baseUri = Request.Url.Scheme + "://" + Request.Url.Authority;
                var redirectUrls = new RedirectUrls()
                {
                    return_url = baseUri + "/Paypal/ExecutePayment?guid=" + guid,
                    cancel_url = baseUri + "/Cart/Index"
                };

                var payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = new List<Transaction>() { transaction },
                    redirect_urls = redirectUrls
                };

                // Gửi lên PayPal
                var createdPayment = payment.Create(apiContext);

                // Lưu Payment ID (String) vào Session
                Session[guid] = createdPayment.id;

                var approvalUrl = createdPayment.links.FirstOrDefault(lnk => lnk.rel.ToLower().Trim().Equals("approval_url"));

                if (approvalUrl != null)
                {
                    return Redirect(approvalUrl.href);
                }
            }
            catch (PayPal.PayPalException ex)
            {
                // Log lỗi chi tiết
                ViewBag.Error = "Lỗi PayPal: " + ex.Message;
                if (ex.InnerException != null) ViewBag.Error += " | " + ex.InnerException.Message;
                return View("FailureView");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
                return View("FailureView");
            }

            return View("FailureView");
        }

        // HÀM NÀY ĐƯỢC GỌI KHI PAYPAL CHUYỂN HƯỚNG VỀ
        public ActionResult ExecutePayment(string guid, string PayerID, string token)
        {
           
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

            string clientId = "AUYJKjiga67n8FlvzV15iimges9rQ4VNKU6VFxTEl68lxundAPwaRWCuD3vn7f-MIEK8wDfr0iUSzpKy";
            string clientSecret = "ENEY74YjB8ixxOaoSj_DqNY8T2JtwAwwVBN72KT2E3rjdbMN8AoJhGEa9lqrMmkl6LIrqA8Z_IjfOxcs";

            var config = new Dictionary<string, string> { { "mode", "sandbox" } };
            string accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            APIContext apiContext = new APIContext(accessToken) { Config = config };
            

            try
            {
                // 2. Lấy PaymentID từ Session
                // (Lưu ý: Hàm trước ta chỉ lưu chuỗi ID, không lưu CheckoutVM nên ta lấy string)
                if (Session[guid] == null)
                {
                    ViewBag.Error = "Phiên thanh toán đã hết hạn hoặc không tìm thấy.";
                    return View("FailureView");
                }

                string paymentId = Session[guid].ToString();

                // 3. Thực thi thanh toán trên PayPal
                var paymentExecution = new PaymentExecution() { payer_id = PayerID };
                var payment = new Payment() { id = paymentId };

                var executedPayment = payment.Execute(apiContext, paymentExecution);

                // 4. Kiểm tra kết quả
                if (executedPayment.state.ToLower() != "approved")
                {
                    ViewBag.Error = "Thanh toán bị từ chối bởi PayPal.";
                    return View("FailureView");
                }

                // =========================================================================
                // 5. THANH TOÁN THÀNH CÔNG -> LƯU VÀO DATABASE
                // =========================================================================

                // Lấy thông tin khách hàng đang đăng nhập
                // (Đoạn này giả định bạn đã đăng nhập, nếu chưa thì phải check null)
                var customerName = User.Identity.Name;
                var customer = db.Customers.SingleOrDefault(c => c.Username == customerName);

                // Nếu không tìm thấy khách (lỗi session), ta lấy khách mặc định hoặc báo lỗi
                // Ở đây tôi để tạm logic an toàn:
                int customerId = (customer != null) ? customer.CustomerID : 0;
                // Sửa .Address thành .CustomerAddress
                string deliveryAddr = (customer != null) ? customer.CustomerAddress : "Địa chỉ PayPal";

                var cartService = GetCartService();
                var cart = cartService.GetCart();

                // Tạo đơn hàng
                _24DH112073_MyStore.Models.Order order = new _24DH112073_MyStore.Models.Order();
                order.CustomerID = customerId;
                order.OrderDate = DateTime.Now;
                order.TotalAmount = cart.TotalValue();
                order.PaymentStatus = "Đã thanh toán PayPal";
                order.PaymentMethod = "PayPal";
                order.DeliveryMethod = "Giao hàng tiêu chuẩn";
                order.AddressDelivery = deliveryAddr; // Lấy địa chỉ từ hồ sơ khách hàng

                db.Orders.Add(order);
                db.SaveChanges(); // Lưu Order để lấy ID

                // Lưu chi tiết đơn hàng & Trừ kho
                foreach (var item in cart.Items)
                {
                    _24DH112073_MyStore.Models.OrderDetail od = new _24DH112073_MyStore.Models.OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    db.OrderDetails.Add(od);

                    // Trừ số lượng tồn kho
                    var product = db.Products.Find(item.ProductID);
                    if (product != null)
                    {
                        // Kiểm tra nếu kho còn đủ hàng mới trừ
                        if (product.Quantity >= item.Quantity)
                        {
                            product.Quantity -= item.Quantity;
                        }
                        else
                        {
                            product.Quantity = 0; // Hoặc xử lý logic hết hàng
                        }
                        db.Entry(product).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();

                // Xóa giỏ hàng & Session
                cartService.ClearCart();
                Session[guid] = null;

                // Chuyển đến trang thông báo thành công
                // (Bạn có thể redirect về Action OrderSuccess hoặc hiển thị View SuccessView)
                return View("SuccessView");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi thực thi thanh toán: " + ex.Message;
                return View("FailureView");
            }
        }

        // Trang báo lỗi
        public ActionResult FailureView()
        {
            return View();
        }
    }
}