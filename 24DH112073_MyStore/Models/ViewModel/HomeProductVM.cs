using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _24DH112073_MyStore.Models;
using PagedList; 

namespace _24DH112073_MyStore.Models.ViewModel
{
    public class HomeProductVM
    {
        // Tiêu chí để search
        public string SearchTerm { get; set; }

        // Các thuộc tính hỗ trợ phân trang (cho Sản phẩm mới)
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 6; 

        // Danh sách sản phẩm nổi bật 
        public List<Product> FeaturedProducts { get; set; }

        // Danh sách sản phẩm mới 
        public PagedList.IPagedList<Product> NewProducts { get; set; }

        public List<Product> YouMightLikeProducts { get; set; }
    }
}