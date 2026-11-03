using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;
using _24DH112073_MyStore.Models;
using PagedList;

namespace _24DH112073_MyStore.Models.ViewModel
{
    public class AdminUserSearchVM
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
        // Dùng "User" hoặc "AdminUser" 
        public PagedList.IPagedList<User> AdminUsers { get; set; }
    }
}