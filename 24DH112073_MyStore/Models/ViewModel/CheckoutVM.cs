using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using _24DH112073_MyStore.Models;

namespace _24DH112073_MyStore.Models.ViewModel
{
    // Class này lưu thông tin cho form Checkout
    public class CheckoutVM
    {
        public List<CartItem> CartItems { get; set; }
        public int CustomerID { get; set; }
        public string Username { get; set; }
        public System.DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Phương thức giao hàng")]
        [Required(ErrorMessage = "Vui lòng chọn phương thức giao hàng")]
        public string DeliveryMethod { get; set; } // <-- SỬA 1: Đổi tên

        [Display(Name = "Địa chỉ giao hàng")]
        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        public string AddressDelivery { get; set; } // <-- SỬA 2: Đổi tên

        public List<OrderDetail> OrderDetails { get; set; }
    }
}