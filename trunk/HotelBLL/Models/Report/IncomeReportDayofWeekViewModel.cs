using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models.Report
{
    public class IncomeReportDayOfWeekViewModel
    {
        public int CurrentInStay { get; set; }
        public int WaitApproveMoney { get; set; }
        public List<IncomeReportDayOfWeekItemModel> IncomeItems { get; set; }
        public bool IsReportToNow { get; set; }

        public IncomeReportDayOfWeekViewModel()
        {
           IncomeItems = new List<IncomeReportDayOfWeekItemModel>();
        }
    }
}