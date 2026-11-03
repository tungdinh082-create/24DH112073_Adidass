using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH112073_MyStore.Models;
using _24DH112073_MyStore.Models.ViewModel;
using System.Data.Entity;
using PagedList;


namespace _24DH112073_MyStore.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        

        // HÀM LẤY DỊCH VỤ GIỎ HÀNG 
        private CartService GetCartService()
        {
            return new CartService(this.Session);
        }

        // Hiển thị giỏ hàng VÀ các sản phẩm tương tự
        
        public ActionResult Index(int? page)
        {
            var cart = GetCartService().GetCart();

          
            
            var products = db.Products
                             .Include(p => p.Category)
                             .Include(p => p.OrderDetails)
                             .Where(p => p.IsActive == true) 
                             .ToList();
       

            var similarProducts = new List<Product>();

            if (cart.Items != null && cart.Items.Any())
            {
                // 1. NẾU GIỎ HÀNG CÓ SẢN PHẨM: Tải "Sản phẩm tương tự"
                ViewBag.RecommendedTitle = "Sản phẩm tương tự";

                similarProducts = products.Where(p => cart.Items.Any(ci => ci.Category == p.Category.CategoryName)
                                                   && !cart.Items.Any(ci => ci.ProductID == p.ProductID))
                                          .ToList();
            }
            else
            {
                // 2. NẾU GIỎ HÀNG RỖNG: Tải "Sản phẩm đề cử" 
                ViewBag.RecommendedTitle = "Sản phẩm đề cử";

                similarProducts = products
                                  .OrderByDescending(p => p.OrderDetails.Count()) 
                                  .Take(12) // Lấy 12 sản phẩm
                                  .ToList();
            }

            // 3. Phân trang
            int pageNumber = (page ?? 1);
            int pageSize = cart.PageSize; // (Mặc định là 6)

            cart.SimilarProducts = similarProducts.ToPagedList(pageNumber, pageSize);

            return View(cart);
        }

        // GET: Cart/AddToCart/5 (NÂNG CẤP)
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            // 1. Tìm sản phẩm trong DB (bao gồm cả Category) [cite: 338]
            var product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
            {
                // 2. Lấy CartService
                var cartService = GetCartService();

                // 3. Gọi hàm AddItem (trong Cart.cs)
                cartService.GetCart().AddItem(
                    product,
                    quantity,
                    product.Category.CategoryName // Truyền tên Category [cite: 360]
                );
            }

            // 4. Chuyển hướng về trang Giỏ hàng
            return RedirectToAction("Index");
        }

        // GET: Cart/RemoveFromCart/5 (NÂNG CẤP) [cite: 339]
        public ActionResult RemoveFromCart(int productId)
        {
            var cartService = GetCartService();
            cartService.GetCart().RemoveItem(productId);

            return RedirectToAction("Index");
        }

        // GET: Cart/ClearCart (NÂNG CẤP) [cite: 340]
        public ActionResult ClearCart()
        {
            var cartService = GetCartService();
            cartService.GetCart().ClearCart();

            return RedirectToAction("Index");
        }

        // POST: Cart/UpdateQuantity (NÂNG CẤP) [cite: 341]
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            if (quantity > 0)
            {
                var cartService = GetCartService();
                cartService.GetCart().UpdateQuantity(id, quantity);
            }
            return RedirectToAction("Index");
        }

        
    }
}