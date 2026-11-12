IF DB_ID('MyStore') IS NULL CREATE DATABASE [MyStore];
GO
USE [MyStore];
GO
IF OBJECT_ID('dbo.OrderDetail', 'U') IS NOT NULL DROP TABLE dbo.OrderDetail;
IF OBJECT_ID('dbo.[Order]', 'U') IS NOT NULL DROP TABLE dbo.[Order];
IF OBJECT_ID('dbo.Product', 'U') IS NOT NULL DROP TABLE dbo.Product;
IF OBJECT_ID('dbo.Customer', 'U') IS NOT NULL DROP TABLE dbo.Customer;
IF OBJECT_ID('dbo.[User]', 'U') IS NOT NULL DROP TABLE dbo.[User];
IF OBJECT_ID('dbo.Category', 'U') IS NOT NULL DROP TABLE dbo.Category;
IF OBJECT_ID('dbo.post', 'U') IS NOT NULL DROP TABLE dbo.post;
GO
CREATE TABLE dbo.Category (
    CategoryID INT NOT NULL PRIMARY KEY,
    CategoryName NVARCHAR(MAX) NOT NULL
);
CREATE TABLE dbo.[User] (
    Username NVARCHAR(255) NOT NULL PRIMARY KEY,
    Password NCHAR(50) NOT NULL,
    UserRole NVARCHAR(50) NOT NULL
);
CREATE TABLE dbo.Customer (
    CustomerID INT NOT NULL PRIMARY KEY,
    CustomerName NVARCHAR(MAX) NOT NULL,
    CustomerPhone NVARCHAR(15) NOT NULL,
    CustomerEmail NVARCHAR(MAX) NULL,
    CustomerAddress NVARCHAR(MAX) NULL,
    Username NVARCHAR(255) NOT NULL
);
CREATE TABLE dbo.Product (
    ProductID INT NOT NULL PRIMARY KEY,
    CategoryID INT NOT NULL,
    ProductName NVARCHAR(MAX) NOT NULL,
    ProductDescription NVARCHAR(MAX) NOT NULL,
    ProductPrice DECIMAL(18,2) NOT NULL,
    ProductImage NVARCHAR(MAX) NULL,
    Quantity INT NOT NULL DEFAULT 50,
    Color NVARCHAR(100) NULL,
    ProductSize NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
CREATE TABLE dbo.[Order] (
    OrderID INT NOT NULL PRIMARY KEY,
    CustomerID INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    PaymentStatus NVARCHAR(MAX) NULL,
    AddressDelivery NVARCHAR(MAX) NOT NULL,
    DeliveryMethod NVARCHAR(100) NULL,
    PaymentMethod NVARCHAR(100) NULL
);
CREATE TABLE dbo.OrderDetail (
    ID INT NOT NULL PRIMARY KEY,
    ProductID INT NOT NULL,
    OrderID INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalPrice DECIMAL(29,2) NULL
);
CREATE TABLE dbo.post (
    id INT NOT NULL PRIMARY KEY,
    author_id INT NULL,
    title NVARCHAR(MAX) NULL,
    description NVARCHAR(MAX) NULL,
    view_count INT NULL,
    created_at DATETIME NULL,
    published_at DATETIME NULL
);
GO