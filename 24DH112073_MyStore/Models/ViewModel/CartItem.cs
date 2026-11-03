using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _24DH112073_MyStore.Models;

namespace _24DH112073_MyStore.Models.ViewModel
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductImage { get; set; }
        public string Category { get; set; } // Dùng cho Giỏ hàng Nâng cao 

       // Thuộc tính tự tính Tổng giá 
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}