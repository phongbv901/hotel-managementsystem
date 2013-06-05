using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models.ServiceReport
{
    public class ServiceReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<ServiceReportItemModel> ServiceItems { get; set; }

    }

   
}