using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _24DH112073_MyStore.Models; 
using PagedList;

namespace _24DH112073_MyStore.Models.ViewModel
{
    public class ProductDetailsVM
    {
        // 1. Sản phẩm chính đang xem
        public Product Product { get; set; }

        // 2. Số lượng (mặc định là 1)
        public int quantity { get; set; } = 1;

        // 3. Tạm tính (cho form)
        public decimal estimatedValue { get; set; }

        // 4. Phân trang cho "Sản phẩm bán chạy"
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 3; // Slide yêu cầu 3 sản phẩm/trang

        // 5. Danh sách sản phẩm tương tự (cùng danh mục)
        // Slide dùng List<> nhưng code lại dùng IPagedList<>, chúng ta dùng PagedList
        public IPagedList<Product> RelatedProducts { get; set; }

        // 6. Danh sách sản phẩm bán chạy (cùng danh mục)
        public IPagedList<Product> TopProducts { get; set; }
    }
}