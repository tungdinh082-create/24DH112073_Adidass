using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;
using _24DH112073_MyStore.Models;
using PagedList;

namespace _24DH112073_MyStore.Models.ViewModel
{
    public class OrderSearchVM
    {
        public string SearchTerm { get; set; } // Tìm theo tên KH
        public string Status { get; set; } // Lọc theo trạng thái
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10; // 10 đơn hàng mỗi trang
        public PagedList.IPagedList<Order> Orders { get; set; }
    }
}