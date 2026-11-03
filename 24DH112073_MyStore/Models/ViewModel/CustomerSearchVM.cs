using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;
using _24DH112073_MyStore.Models;
using PagedList;

namespace _24DH112073_MyStore.Models.ViewModel
{
    public class CustomerSearchVM
    {
        public string SearchTerm { get; set; }
        public string SortOrder { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
        public PagedList.IPagedList<Customer> Customers { get; set; }

    }

}