using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelBLL.Models.Report;
using HotelDAL;
using HotelBLL.Models.ServiceReport;
using System.Web.Configuration;

namespace HotelManagement.Controllers
{
    [Authorize]
    public class ServiceReportController : Controller
    {
        //
        // GET: /Report/
        HotelDataContext _db = new HotelDataContext(WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString);


        public ActionResult ReportServiceByDay()
        {
            return View("_ServiceDayReport");

        }

        public ActionResult ReportServiceByMonth()
        {
            //Trả về 7 ngày gần nhất
            return View("_ServiceMonthReport");

        }


       #region Day Report
        public ActionResult GetServiceReportFromNow(int totalDay)
        {
            DateTime toDate = DateTime.Now;
            DateTime fromDate = toDate.AddDays(-totalDay + 1);
            ServiceReportViewModel model = new ServiceReportViewModel();
            model.FromDate = fromDate;
            model.ToDate = toDate;
            model.ServiceItems  = GetServiceReportInRange(fromDate, toDate);
            return PartialView("_ServiceDayReportSearchList", model);
        }

        public ActionResult GetServiceReport(DateTime fromDate, DateTime toDate)
        {
            ServiceReportViewModel model = new ServiceReportViewModel();
            model.FromDate = fromDate;
            model.ToDate = toDate;
            model.ServiceItems = GetServiceReportInRange(fromDate, toDate);
            return PartialView("_ServiceDayReportSearchList", model);
        }

#endregion

        #region Month Report
        public ActionResult GetServiceMonthReportFromNow(int totalMonth)
        {
            DateTime toDate = DateTime.Now;
            DateTime fromDate = toDate.AddMonths(-totalMonth + 1);
            ServiceReportViewModel model = new ServiceReportViewModel();
            model.FromDate = fromDate;
            model.ToDate = toDate;
            model.ServiceItems = GetServiceMonthInRange(fromDate, toDate);
            return PartialView("_ServiceMonthReportSearchList", model);
        }

        public ActionResult GetServiceMonthReport(DateTime fromDate, DateTime toDate)
        {
            ServiceReportViewModel model = new ServiceReportViewModel();
            model.FromDate = fromDate;
            model.ToDate = toDate;
            model.ServiceItems = GetServiceMonthInRange(fromDate, toDate);
            return PartialView("_ServiceMonthReportSearchList", model);
        }

        #endregion
      
#region Private Method

        private TimeSpan GetStartTimeOfDay()
        {
            TimeSpan timeOfDay = new TimeSpan(0, 0, 0, 0);

            var hotelInfo = _db.HotelInfos.First();

            if (hotelInfo != null)
            {
                timeOfDay = hotelInfo.StartDayTime.TimeOfDay;
            }
            return timeOfDay;
        }

        /// <summary>
        /// Get all income in Day
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private List<ServiceReportItemModel> GetServiceReportInRange(DateTime fromDate,DateTime toDate)
        {

            var timeOfDay = GetStartTimeOfDay();
            fromDate = fromDate.Date;
            toDate = toDate.Date;

           var  searchFromDate = fromDate + timeOfDay;
          var   searchToDate = toDate.AddDays(1) + timeOfDay;

            var minusTimeOfDay = -timeOfDay;

            var products = _db.Products.ToList();

            List<ServiceReportItemModel> serviceReport = new List<ServiceReportItemModel>();

            foreach (var product in products)
            {
                ServiceReportItemModel item = new ServiceReportItemModel();
                
                item.Product = product;
                int productId = product.ProductID;
                var productQuantity = (from order in _db.OrderDetails
                                        where order.ProductID == productId &&
                                            order.OrderDate >= searchFromDate && order.OrderDate <= searchToDate
                                        let dt = order.OrderDate.Add(minusTimeOfDay).Date
                                        group order by new {y = dt.Year, m = dt.Month, d = dt.Day}
                                        into collections
                                        select new ServiceReportItemModel.ProductQuantityInDay()
                                                   {

                                                       Date =
                                                           new DateTime(collections.Key.y, collections.Key.m,
                                                                        collections.Key.d),
                                                       Quantity = collections.Sum(r => r.Quantity),

                                                   }).ToList();
                DateTime date = fromDate;
                int i = 0;
                while (date <= toDate)
                {
                    bool isAdd = true;
                    if (i < productQuantity.Count)
                    {
                        DateTime reportDate = productQuantity.ElementAt(i).Date.Date;
                        if (date == reportDate)
                        {
                            isAdd = false;
                        }

                    }

                    if (isAdd)
                    {
                        ServiceReportItemModel.ProductQuantityInDay emptyItem = new ServiceReportItemModel.ProductQuantityInDay();
                        emptyItem.Date = date;
                        productQuantity.Insert(i, emptyItem);
                    }

                    i++;
                    date = date.AddDays(1);
                }


                item.QuantityInDays = productQuantity;
                serviceReport.Add(item);

            }


           

            return serviceReport;


          
        }



        private List<ServiceReportItemModel> GetServiceMonthInRange(DateTime fromDate, DateTime toDate)
        {

            var timeOfDay = GetStartTimeOfDay();

            fromDate = new DateTime(fromDate.Year, fromDate.Month, 1);//Lay tu dau thang
            toDate = new DateTime(toDate.Year, toDate.Month + 1, 1).Add(timeOfDay);//Lay ngay dau thang sau

            var minusTimeOfDay = -timeOfDay;

            var products = _db.Products.ToList();

            List<ServiceReportItemModel> serviceReport = new List<ServiceReportItemModel>();

            foreach (var product in products)
            {
                ServiceReportItemModel item = new ServiceReportItemModel();

                item.Product = product;
                int productId = product.ProductID;
                var productQuantity = (from order in _db.OrderDetails
                                       where order.ProductID == productId &&
                                           order.OrderDate >= fromDate && order.OrderDate <= toDate
                                       let dt = order.OrderDate.Add(minusTimeOfDay).Date
                                       group order by new { y = dt.Year, m = dt.Month}
                                           into collections
                                           select new ServiceReportItemModel.ProductQuantityInDay()
                                           {

                                               Date =
                                                   new DateTime(collections.Key.y, collections.Key.m,
                                                                1),
                                               Quantity = collections.Sum(r => r.Quantity),

                                           }).ToList();
                DateTime date = fromDate.Date;
                int i = 0;

                var newToDate = toDate.Add(minusTimeOfDay);
                while (date < newToDate)
                {
                    bool isAdd = true;
                    if (i < productQuantity.Count)
                    {
                        DateTime reportDate = productQuantity.ElementAt(i).Date.Date;
                        if (date == reportDate)
                        {
                            isAdd = false;
                        }

                    }

                    if (isAdd)
                    {
                        ServiceReportItemModel.ProductQuantityInDay emptyItem = new ServiceReportItemModel.ProductQuantityInDay();
                        emptyItem.Date = date;
                        productQuantity.Insert(i, emptyItem);
                    }

                    i++;
                    date = date.AddMonths(1);
                }



                item.QuantityInDays = productQuantity;
                serviceReport.Add(item);

            }
            return serviceReport;



        }

#endregion
    }
}
