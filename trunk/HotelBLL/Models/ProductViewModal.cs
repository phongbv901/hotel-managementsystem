using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelDAL;

namespace HotelBLL.Models
{
    public class ProductViewModal
    {
        public IEnumerable<HotelDAL.Product> products { get; set; }
        public IEnumerable<HotelDAL.ProductItem> item { get; set; }
        public IEnumerable<HotelDAL.ProductCategory> category { get; set; }
        
    }
}
