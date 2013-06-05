using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models.Report
{
    public class IncomeReportMonthViewModel
    {
        public int CurrentInStay { get; set; }
        public int WaitApproveMoney { get; set; }
        public List<IncomeReportMonthItemModel> IncomeItems { get; set; }
        public bool IsReportToNow { get; set; }

        public IncomeReportMonthViewModel()
        {
            IncomeItems = new List<IncomeReportMonthItemModel>();
        }
    }
}