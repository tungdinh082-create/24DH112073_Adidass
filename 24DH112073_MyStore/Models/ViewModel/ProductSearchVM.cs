using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;
using _24DH112073_MyStore.Models;

namespace _24DH112073_MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        // Tiêu chí tìm kiếm (tên, mô tả, chủng loại)
        public string SearchTerm { get; set; }

        // Tiêu chí tìm kiếm theo khoảng giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Thứ tự sắp xếp
        public string SortOrder { get; set; }

        // Các thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; } // Trang hiện tại [cite: 361]
        public int PageSize { get; set; } = 10; // Số sản phẩm mỗi trang [cite: 361]

        // Danh sách sản phẩm thỏa điều kiện tìm kiếm (dạng phân trang)
        public PagedList.IPagedList<Product> Products { get; set; }
    }
}