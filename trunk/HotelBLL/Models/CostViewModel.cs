using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class CostViewModel
    {
        public Cost Cost { get; set; }
        public IEnumerable<CostCategory> CostCategories { get; set; }
        public int Index { get; set; }
    }
}