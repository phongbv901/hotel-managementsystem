using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class CostItemViewModel
    {
        public Cost Cost { get; set; }
        public bool IsAdmin { get; set; }
        public int Index { get; set; }
    }
}