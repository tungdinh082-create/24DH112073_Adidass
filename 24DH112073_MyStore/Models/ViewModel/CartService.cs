using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _24DH112073_MyStore.Models.ViewModel
{
    // Class này quản lý việc lưu/lấy Giỏ hàng từ Session
    public class CartService
    {
        // Tên của Session
        private const string CartSession = "CartSession";

        // Lấy Session hiện tại
        private readonly HttpSessionStateBase session;

        // Hàm khởi tạo (Constructor)
        public CartService(HttpSessionStateBase session)
        {
            this.session = session;
        }

        // Hàm lấy giỏ hàng (GetCart)
        public Cart GetCart()
        {
            // 1. Thử lấy giỏ hàng từ Session
            var cart = (Cart)session[CartSession];

            // 2. Nếu chưa có giỏ hàng (lần đầu truy cập)
            if (cart == null)
            {
                // Tạo mới một giỏ hàng rỗng
                cart = new Cart();
                // Lưu giỏ hàng mới vào Session
                session[CartSession] = cart;
            }

            return cart;
        }

        // Hàm xóa giỏ hàng (ClearCart)
        public void ClearCart()
        {
            session[CartSession] = null; // Xóa Session
        }
    }
}