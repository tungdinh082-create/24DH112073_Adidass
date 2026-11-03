using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace _24DH112073_MyStore.Models
{
    // 1. Đây là class chứa các quy tắc và thông báo lỗi
    public class OrderMetaData
    {
        [Display(Name = "Ngày đặt")]
        [Required(ErrorMessage = "Ngày đặt là bắt buộc")] // <-- Sửa lỗi "OrderDate field is required"
        [DataType(DataType.Date, ErrorMessage = "Ngày đặt không hợp lệ")]
        public System.DateTime OrderDate { get; set; }

        [Display(Name = "Tổng tiền")]
        [Required(ErrorMessage = "Tổng tiền là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tổng tiền phải là một số dương")] // <-- Sửa lỗi "must be a number"
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Địa chỉ giao")]
        public string AddressDelivery { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public int CustomerID { get; set; }
    }

    // 2. Áp dụng class Metadata này vào Model "Order" (được sinh tự động)
    [MetadataType(typeof(OrderMetaData))]
    public partial class Order
    {
        // Để trống class này, nó sẽ tự động liên kết
    }
}