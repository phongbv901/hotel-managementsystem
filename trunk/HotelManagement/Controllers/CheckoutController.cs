using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelBLL.Models;
using HotelFee;
using HotelDAL;
using System.Web.Configuration;
namespace HotelManagement.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        //
        // GET: /Checkout/
        private string connectionString;
        private HotelDataContext _db;
        public  CheckoutController()
        {
            connectionString = WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString;
            HotelDataContext _db = new HotelDataContext(connectionString);
        }
        public ActionResult Index(int rentId)
        {
            Rent rent = _db.Rents.Where(r => r.RentID == rentId).Single();

           
                if (rent.CheckOutDate == null)
                {
                    rent.CheckOutDate = DateTime.Now;
                }

                RentDetailViewModel checkoutModelView = new RentDetailViewModel();
                checkoutModelView.IsNew = true;
                checkoutModelView.Rent = rent;

            PriceCalculator calculator = PriceFactory.GetPriceCalculator((RentTypeEnum) rent.RentType,_db.Connection.ConnectionString);
            rent.RentFee = calculator.CalculatePrice(rent.CheckInDate, rent.CheckOutDate.Value, rent.Room.PriceGroupID);
            rent.FeeDescription = calculator.Description;
               

                int orderFee = 0;
                foreach (var item in rent.OrderDetails)
                {
                    orderFee += item.TotalPrice;
                }

                rent.OrderFee = orderFee;
                //_db.SubmitChanges();
                return PartialView("_Checkout", checkoutModelView);
           
        }
        public ActionResult UpdateRentInfo(int rentID, int rentType, DateTime checkInDate, DateTime checkOutDate)
        {
            var rent = _db.Rents.Single(r => r.RentID == rentID);
            int priceGroupID = rent.Room.PriceGroupID;
            // calculate
           PriceCalculator priceCalculator =  PriceFactory.GetPriceCalculator((RentTypeEnum)rentType,connectionString);
            int rentFee = priceCalculator.CalculatePrice(checkInDate, checkOutDate, priceGroupID);
            string description = priceCalculator.Description;
           return Json(new { rentFee = rentFee, description = description }, JsonRequestBehavior.AllowGet); 
        }
        
        



        


        /// <summary>
        /// Handle event when user checkout rent.
        /// </summary>
        /// <param name="rentID"></param>
        /// <param name="checkInDate"></param>
        /// <param name="checkOutDate"></param>
        /// <param name="rentType"></param>
        /// <param name="isPayAll"></param>
        /// <returns></returns>
        public ActionResult CheckOut(int rentID,DateTime checkInDate,DateTime checkOutDate,int rentType, bool isPayAll)
        {
            var rent = _db.Rents.Single(r => r.RentID == rentID);
            rent.CheckOutDate = checkOutDate;

            RentTypeEnum mode = (RentTypeEnum)Enum.Parse(typeof(RentTypeEnum), Enum.GetName(typeof(RentTypeEnum),rentType));
            rent.RentType = (int)mode;

            PriceCalculator calc = PriceFactory.GetPriceCalculator(mode,connectionString);
            rent.RentFee = calc.CalculatePrice(checkInDate,checkOutDate, rent.Room.PriceGroupID);
            rent.FeeDescription = calc.Description;
            rent.OrderFee = rent.OrderDetails.Sum(ord => ord.TotalPrice);
            
            if (isPayAll)
            {
                int paymentAmount = rent.OrderFee + rent.RentFee - rent.TotalPayment;
                if (paymentAmount > 0)
                {
                    var aPayment = new Payment();
                    aPayment.Amount = paymentAmount;
                    aPayment.PayTime = DateTime.Now;
                    if (HttpContext.User.Identity.IsAuthenticated)
                    {
                        aPayment.Username = HttpContext.User.Identity.Name;
                    }
                    aPayment.RentID = rent.RentID;

                    rent.Payments.Add(aPayment);
                    rent.TotalPayment += paymentAmount;

                }
            }
            
            
            rent.RentStatus = (int)RentStatusEnum.Paid;
            //checkout person.
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                rent.CheckOutPerson = HttpContext.User.Identity.Name;
            }
            rent.Room.RoomStatus = 1;


            _db.SubmitChanges();

            //return RedirectToAction("Summary", new { txtRentID = aRent.RentID });
            return View("_CheckoutSummary", rent);

        }


        public ActionResult Summary(int rentID)
        {
            var rent = _db.Rents.Single(r => r.RentID == rentID);
            return View("_CheckoutSummary", rent);
        }


        
    }
}
