using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class ProductImportViewModel
    {
        public ProductInventory ProductInventory { get; set; }
        public int Index { get; set; }
        
    }
}