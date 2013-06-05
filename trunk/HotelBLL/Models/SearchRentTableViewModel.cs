using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class SearchRentTableViewModel
    {
        public IEnumerable<HotelDAL.Rent> Rents { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPageIndex { get; set; }
        public int Status { get; set; }

        public bool IsAdmin { get; set; }

    }
}