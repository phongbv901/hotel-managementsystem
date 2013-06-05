using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class MenuViewModel
    {
        public int RentID { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}