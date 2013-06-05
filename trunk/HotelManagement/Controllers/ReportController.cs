using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelDAL;
using HotelBLL.Models.Report;
using System.Web.Configuration;

namespace HotelManagement.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        HotelDataContext _db = new HotelDataContext(WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString);
        public ActionResult ReportIncomeMonth()
        {
            //Trả về 7 ngày gần nhất
            return View("_IncomeMonthReport");

        }

        public ActionResult ReportIncomeDayOfWeek()
        {
            //Trả về 7 ngày gần nhất
            return View("_IncomeDayofWeekReport");

        }

        public ActionResult ReportIncomeDay()
        {
            //Trả về 7 ngày gần nhất
            return View("_IncomeDayReport");

        }


        #region Day Report
        public ActionResult GetDayIncomeReport(DateTime fromDate, DateTime toDate)
        {
            IncomeReportViewModel model = new IncomeReportViewModel();
            
            
            IEnumerable<IncomeReportItemModel> itemList = GetIncomeOfDayList(fromDate,toDate);
            model.IncomeItems = itemList.ToList();
            model.IsReportToNow = false;


            return PartialView("_IncomeDayReportSearchList", model);
        }



        public ActionResult GetDayIncomeReportFromNow(int totalDay)
        {
            IncomeReportViewModel model = new IncomeReportViewModel();

            DateTime toDate = DateTime.Now;
            DateTime fromDate = toDate.AddDays(-totalDay+1);
            IEnumerable<IncomeReportItemModel> itemList = GetIncomeOfDayList(fromDate, toDate);
            model.IncomeItems = itemList.ToList();
            model.IsReportToNow = true;


            return PartialView("_IncomeDayReportSearchList", model);
        }


        #endregion

        #region DayOfWeek Report
        public ActionResult GetDayOfWeekIncomeReport(DateTime fromDate, DateTime toDate)
        {
            IncomeReportDayOfWeekViewModel model = new IncomeReportDayOfWeekViewModel();


            IEnumerable<IncomeReportDayOfWeekItemModel> itemList = GetIncomeOfDayInWeek(fromDate, toDate);
            model.IncomeItems = itemList.ToList();
            model.IsReportToNow = false;


            return PartialView("_IncomeDayOfWeekReportSearchList", model);
        }



        public ActionResult GetDayOfWeekIncomeReportFromNow(int totalMonth)
        {
            IncomeReportDayOfWeekViewModel model = new IncomeReportDayOfWeekViewModel();

            DateTime toDate = DateTime.Now;
            DateTime fromDate = toDate.AddMonths(-totalMonth);
            IEnumerable<IncomeReportDayOfWeekItemModel> itemList = GetIncomeOfDayInWeek(fromDate, toDate);
            model.IncomeItems = itemList.ToList();
            model.IsReportToNow = true;


            return PartialView("_IncomeDayOfWeekReportSearchList", model);
        }

        
#endregion  

        #region Month Report
        public ActionResult GetMonthIncomeReport(DateTime fromDate, DateTime toDate)
        {
            IncomeReportViewModel model = new IncomeReportViewModel();


            IEnumerable<IncomeReportItemModel> itemList = GetIncomeOfMonthList(fromDate, toDate);
            model.IncomeItems = itemList.ToList();
            model.IsReportToNow = false;


            return PartialView("_IncomeMonthReportSearchList", model);
        }



        public ActionResult GetMonthIncomeReportFromNow(int totalMonth)
        {
            IncomeReportViewModel model = new IncomeReportViewModel();

            DateTime toDate = DateTime.Now;
            DateTime fromDate = toDate.AddMonths(-totalMonth +1);//+1 for current month
            IEnumerable<IncomeReportItemModel> itemList = GetIncomeOfMonthList(fromDate, toDate);
            model.IncomeItems = itemList.ToList();
            model.IsReportToNow = true;


            return PartialView("_IncomeMonthReportSearchList", model);
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
        private IncomeReportItemModel GetIncomeOfDay(DateTime dateTime, TimeSpan timeOfDay)
        {
            var endDate = dateTime.Date + timeOfDay;
            var startDate = endDate.AddDays(-1);
            var income = (from rent1 in _db.Rents
                          where (rent1.RentStatus == (int) RentStatusEnum.Approved)
                            && rent1.CheckOutDate != null
                          && rent1.CheckOutDate.Value >= startDate
                          && rent1.CheckOutDate.Value <= endDate
                          group rent1 by 1
                          into collections
                          select
                              new IncomeReportItemModel()
                              {
                                  ReportDate = dateTime,
                                      RentCount = collections.Count(),
                                      OrderFee = collections.Sum(r => r.OrderFee),
                                      RentFee = collections.Sum(r => r.RentFee),
                                      ApprovePayement = collections.Sum(r => r.TotalPayment),
                                  }).FirstOrDefault();
            int unApproveMoney =
                _db.Rents.Where(
                    r => (r.RentStatus == (int) RentStatusEnum.Paid && r.CheckOutDate.Value.Date == dateTime.Date)).Sum(
                        r => (int?)r.TotalPayment) ??0;// if null is 0   

            if (income == null)
            {
                income = new IncomeReportItemModel();
            }
            income.UnApprovePayment = unApproveMoney;
            
            return  income;
            
        }

        /// <summary>
        /// Get all income from date to date
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        private IncomeReportItemModel GetIncomeOfDate(DateTime fromDate, DateTime toDate)
        {
            var timeOfDay = GetStartTimeOfDay();

            fromDate = fromDate.Date + timeOfDay;
            toDate = toDate.Date + timeOfDay;
                var income = (from rent1 in _db.Rents
                             where rent1.RentStatus == (int) RentStatusEnum.Approved &&
                                   rent1.CheckOutDate.Value.Date >= fromDate &&
                                   rent1.CheckOutDate.Value.Date <= toDate
                             group rent1 by 1
                             into collections
                             select
                                 new IncomeReportItemModel()
                                     {
                                         RentCount = collections.Count(),
                                         OrderFee = collections.Sum(r => r.OrderFee),
                                         RentFee = collections.Sum(r => r.RentFee),
                                         ApprovePayement = collections.Sum(r => r.TotalPayment)
                                     }).FirstOrDefault();
                                     
            int unApproveMoney =
                _db.Rents.Where(
                    r => (r.RentStatus == (int)RentStatusEnum.Paid && r.CheckOutDate.Value.Date >= fromDate.Date && r.CheckOutDate.Value.Date <= toDate.Date)).Sum(
                        r => (int?)r.TotalPayment)??0;// if null is 0

                if (income == null)
                {
                 income = new IncomeReportItemModel();
                }
            income.UnApprovePayment = unApproveMoney;

            return income;
        }


        private IEnumerable<IncomeReportItemModel> GetIncomeOfDayList(DateTime fromDate, DateTime toDate)
        {
            
            var timeOfDay = GetStartTimeOfDay();

            fromDate = fromDate.Date + timeOfDay;
            toDate = toDate.Date.AddDays(1) + timeOfDay;

            var minusTimeOfDay = -timeOfDay;

            

            List<IncomeReportItemModel> incomeList = (from rent in  _db.Rents 
                                                             where rent.RentStatus == (int) RentStatusEnum.Approved &&
                                                             rent.CheckOutDate != null &&
                                                             rent.CheckOutDate.Value >= fromDate && rent.CheckOutDate.Value <= toDate
                                                             let dt = rent.CheckOutDate.Value.Add(minusTimeOfDay).Date
                                                             group rent by  new { y =dt.Year, m = dt.Month, d = dt.Day} into collections
                                                             select new IncomeReportItemModel()
                                                                        {

                                                                            ReportDate = new DateTime(collections.Key.y, collections.Key.m, collections.Key.d),
                                                                            RentCount = collections.Count(),
                                                                             OrderFee = collections.Sum(r => r.OrderFee),
                                                                             RentFee = collections.Sum(r => r.RentFee),
                                                                             ApprovePayement = collections.Sum(r => r.TotalPayment)
                                                                            
                                                                        }).ToList();


            DateTime date = fromDate.Date;
            int i = 0;
           
            while (date < toDate.Date)
            {
                bool isAdd = true;
                if (i < incomeList.Count)
                {
                    DateTime reportDate = incomeList.ElementAt(i).ReportDate.Date;
                    if ( date == reportDate)
                    {
                        isAdd = false;
                    }
                   
                }
               
                if (isAdd)
                {
                    IncomeReportItemModel emptyItem = new IncomeReportItemModel();
                    emptyItem.ReportDate = date;
                    incomeList.Insert(i, emptyItem);
                }

                i++;
                date = date.AddDays(1);
            }
            

            return incomeList;

        }



        private IEnumerable<IncomeReportDayOfWeekItemModel> GetIncomeOfDayInWeek(DateTime fromDate, DateTime toDate)
        {
            //Report theo thứ
            //Thứ
            //Số ngày
            //tổng thu
            //Bình quân
            var timeOfDay = GetStartTimeOfDay();

            fromDate = fromDate.Date + timeOfDay;
            toDate = toDate.Date.AddDays(1) + timeOfDay;

            var minusTimeOfDay = -timeOfDay;



            List<IncomeReportDayOfWeekItemModel> incomeList = (from rent in _db.Rents
                                                      where rent.RentStatus == (int)RentStatusEnum.Approved &&
                                                      rent.CheckOutDate != null &&
                                                      rent.CheckOutDate.Value >= fromDate && rent.CheckOutDate.Value <= toDate
                                                      let dt = rent.CheckOutDate.Value.Add(minusTimeOfDay).Date
                                                      group rent by new { day = dt.DayOfWeek } into collections
                                                      orderby  collections.Key.day
                                                      select new IncomeReportDayOfWeekItemModel()
                                                      {
                                                          ReportDate =  collections.Key.day,
                                                          RentCount = collections.Count(),
                                                          OrderFee = collections.Sum(r => r.OrderFee),
                                                          RentFee = collections.Sum(r => r.RentFee),
                                                          ApprovePayement = collections.Sum(r => r.TotalPayment)

                                                      }
                                                      ).ToList();

            DayOfWeek dayofWeek = DayOfWeek.Sunday ;
            int i = 0;
            while (dayofWeek <= DayOfWeek.Saturday)
            {
                bool isAdd = true;
                if (i < incomeList.Count)
                {
                   
                    if (incomeList.ElementAt(i).ReportDate == dayofWeek)
                    {
                        isAdd = false;
                    }

                }

                if (isAdd)
                {
                    IncomeReportDayOfWeekItemModel emptyItem = new IncomeReportDayOfWeekItemModel();
                    emptyItem.ReportDate = dayofWeek;
                    incomeList.Insert(i, emptyItem);
                }

                i++;
                dayofWeek = dayofWeek + 1;
            }
            
            
            
            
            
            
            int totalDays = (int)(toDate - fromDate).TotalDays; // Đã +1 ở trên
            int weeks = totalDays/7;//Lấy phần nguyên là số tuần
            int remain = totalDays%7;

            DayOfWeek startDayOfWeek = fromDate.DayOfWeek;

           

            foreach (var item in incomeList)
            {
                int delta = item.ReportDate - startDayOfWeek;
                delta = (delta < 0) ? delta + 7 : delta;
                item.TotalDays = (delta < remain) ? weeks + 1 : weeks;

            }

            return incomeList;

        }



        private IEnumerable<IncomeReportItemModel> GetIncomeOfMonthList(DateTime fromDate, DateTime toDate)
        {

            var timeOfDay = GetStartTimeOfDay();

            fromDate = new DateTime(fromDate.Year,fromDate.Month,1);//Lay tu dau thang
            toDate = new DateTime(toDate.Year,toDate.Month+1,1).Add(timeOfDay);//Lay ngay dau thang sau

            var minusTimeOfDay = -timeOfDay;



            List<IncomeReportItemModel> incomeList = (from rent in _db.Rents
                                                         where rent.RentStatus == (int)RentStatusEnum.Approved &&
                                                         rent.CheckOutDate != null &&
                                                         rent.CheckOutDate.Value >= fromDate && rent.CheckOutDate.Value <= toDate
                                                         let dt = rent.CheckOutDate.Value.Add(minusTimeOfDay).Date
                                                         group rent by new { y = dt.Year, m = dt.Month} into collections
                                                      select new IncomeReportItemModel()
                                                         {

                                                             ReportDate = new DateTime(collections.Key.y, collections.Key.m, 1),
                                                             RentCount = collections.Count(),
                                                             OrderFee = collections.Sum(r => r.OrderFee),
                                                             RentFee = collections.Sum(r => r.RentFee),
                                                             ApprovePayement = collections.Sum(r => r.TotalPayment)

                                                         }).ToList();


            DateTime date = fromDate.Date;
            int i = 0;

            toDate = toDate.Add(minusTimeOfDay);
            while (date < toDate)
            {
                bool isAdd = true;
                if (i < incomeList.Count)
                {
                    DateTime reportDate = incomeList.ElementAt(i).ReportDate.Date;
                    if (date == reportDate)
                    {
                        isAdd = false;
                    }

                }

                if (isAdd)
                {
                    IncomeReportItemModel emptyItem = new IncomeReportItemModel();
                    emptyItem.ReportDate = date;
                    incomeList.Insert(i, emptyItem);
                }

                i++;
                date = date.AddMonths(1);
            }


            return incomeList;

        }

#endregion
    }
}
