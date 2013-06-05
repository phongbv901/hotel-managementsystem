using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBLL.Models
{
    public class SearchCostTableViewModel
    {
        public IEnumerable<HotelDAL.Cost> Costs { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPageIndex { get; set; }
        public int Status { get; set; }

        public bool IsAdmin { get; set; }
    }
}