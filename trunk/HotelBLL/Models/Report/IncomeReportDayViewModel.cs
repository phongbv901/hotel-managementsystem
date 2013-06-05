using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models.Report
{
    public class IncomeReportViewModel
    {
        public int CurrentInStay { get; set; }
        public int WaitApproveMoney { get; set; }
        public List<IncomeReportItemModel> IncomeItems { get; set; }
        public bool IsReportToNow { get; set; }

        public IncomeReportViewModel()
        {
           IncomeItems = new List<IncomeReportItemModel>();
        }
    }
}