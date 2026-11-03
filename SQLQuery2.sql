ALTER TABLE Product
ADD Color NVARCHAR(100); -- Thêm cột Màu sắc
GO

ALTER TABLE Product
ADD ProductSize NVARCHAR(100); -- Thêm cột Kích cỡ (Dùng "ProductSize" để tránh trùng từ khóa "Size")
GO

PRINT 'Đã thêm 2 cột mới (Color, ProductSize) vào bảng Product.';