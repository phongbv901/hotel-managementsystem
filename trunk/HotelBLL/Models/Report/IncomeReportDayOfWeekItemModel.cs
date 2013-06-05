using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBLL.Models.Report
{
    public class IncomeReportDayOfWeekItemModel
    {
        public DayOfWeek ReportDate { get; set; }
        public int TotalDays { get; set; }
        public int RentFee { get; set; }// Tiền thuê phòng
        public int OrderFee { get; set; }//Tiền dịch vụ
        public int RentCount { get; set; }//Số lần thuê
        public int UnApprovePayment { get; set; }// Tiền khách trả chưa Approve
        public int ApprovePayement { get; set; }// Tiền khách trả đã Approve
    }
}