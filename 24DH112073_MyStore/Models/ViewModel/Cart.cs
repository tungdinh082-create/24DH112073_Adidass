using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Web.UI.WebControls;

namespace _24DH112073_MyStore.Models.ViewModel
{
    public class Cart
    {
        // Danh sách các món hàng
        private List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items => items;

        // Chức năng Thêm sản phẩm vào giỏ [cite: 801]
        public void AddItem(Product product, int quantity, string categoryName)
        {
            var existingItem = items.FirstOrDefault(i => i.ProductID == product.ProductID);

            if (existingItem == null)
            {
                // Nếu sản phẩm chưa có, thêm mới
                items.Add(new CartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    ProductImage = product.ProductImage,
                    UnitPrice = product.ProductPrice,
                    Quantity = quantity,
                    Category = categoryName // Thêm danh mục (cho Giỏ hàng nâng cao) [cite: 834]
                });
            }
            else
            {
                // Nếu đã có, cộng thêm số lượng
                existingItem.Quantity += quantity;
            }
        }

        // Chức năng Xóa sản phẩm khỏi giỏ [cite: 801]
        public void RemoveItem(int productId)
        {
            items.RemoveAll(i => i.ProductID == productId);
        }

        // Chức năng Tính tổng giá trị giỏ hàng [cite: 801]
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }

        // Chức năng Làm rỗng giỏ hàng [cite: 801]
        public void ClearCart()
        {
            items.Clear();
        }

        // Chức năng Cập nhật số lượng [cite: 801]
        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }

        // === CÁC THUỘC TÍNH CHO GIỎ HÀNG NÂNG CAO (Thêm từ slide 832) ===

        // Thuộc tính để Nhóm sản phẩm theo Danh mục [cite: 836]
        public List<IGrouping<string, CartItem>> GroupedItems => items.GroupBy(i => i.Category).ToList();

        // Các thuộc tính hỗ trợ phân trang (cho Sản phẩm tương tự) [cite: 837]
        public int PageNumber { get; set; } = 1; // Trang hiện tại
        public int PageSize { get; set; } = 6; // Số sản phẩm mỗi trang

        // Danh sách sản phẩm tương tự (cùng danh mục với các sản phẩm trong giỏ) [cite: 837]
        public PagedList.IPagedList<Product> SimilarProducts { get; set; }
    }
}