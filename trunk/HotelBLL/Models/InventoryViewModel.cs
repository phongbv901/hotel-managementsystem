using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class InventoryViewModel
    {
        public IEnumerable<InventoryHistory> InventoryHistoryList { get; set; } 
        public IEnumerable<ProductItemViewModel> ProductItems { get; set; }
        public DateTime ReportDate { get; set; }
    }
}