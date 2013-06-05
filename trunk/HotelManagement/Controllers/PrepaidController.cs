using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelDAL;
using HotelFee;
using HotelBLL.Models;
using System.Web.Configuration;

namespace HotelManagement.Controllers
{
    [Authorize]
    public class PrepaidController : Controller
    {
        //
        // GET: /Prepaid/
         private string connectionString;
         HotelDataContext db;
        public PrepaidController()
        {
            connectionString = WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString;
            db = new HotelDataContext(connectionString);
        }
        public ActionResult Index(int txtRentID)
        {
            Rent rent = db.Rents.Where(r => r.RentID == txtRentID).Single();
            if (rent.CheckOutDate == null)
            {
                rent.CheckOutDate = DateTime.Now;
            }
            PrepaidViewModel prepaidViewModel = new PrepaidViewModel();
            prepaidViewModel.Rent = rent;
            //prepaidViewModel.RentTypes = _db.RentTypes.ToList();

            prepaidViewModel.CurrentDate = DateTime.Now;

            int rentTypeID = rent.RentType;
            RentTypeEnum mode = (RentTypeEnum)Enum.Parse(typeof(RentTypeEnum), Enum.GetName(typeof(RentTypeEnum), rentTypeID));

            PriceCalculator priceCalculator =
                PriceFactory.GetPriceCalculator(mode,connectionString);
            rent.RentFee = priceCalculator.CalculatePrice(rent.CheckInDate, prepaidViewModel.CurrentDate, (int)rent.Room.PriceGroupID);

            int orderFee = 0;
            foreach (var item in rent.OrderDetails)
            {
                orderFee += item.TotalPrice;
            }

            rent.OrderFee = orderFee;
            return PartialView("_Prepaid", prepaidViewModel);
        }
        public ActionResult UpdateInfo(int txtRentID,int txtRoomID, int txtRentTypeID, DateTime txtCheckInDate, DateTime txtCheckOutDate)
        {
            var rent = db.Rents.Single(r => r.RentID == txtRentID);
            var room = rent.Room;
            int priceGroupID = room.PriceGroupID;
            // calculate
            RentTypeEnum mode =
                (RentTypeEnum)Enum.Parse(typeof(RentTypeEnum), Enum.GetName(typeof(RentTypeEnum), txtRentTypeID));
            PriceCalculator priceCalculator =
                PriceFactory.GetPriceCalculator(mode,connectionString);
            var price = priceCalculator.CalculatePrice(txtCheckInDate, txtCheckOutDate, priceGroupID);

            //Update thong tin thue
            rent.RentType = txtRentTypeID;
            rent.CheckOutDate = txtCheckOutDate;
            rent.RentFee = price;
            db.SubmitChanges();

            return Json(new { rentFee = price}, JsonRequestBehavior.AllowGet); 
        }
        //[HttpPost]
        //public ActionResult Prepaid(int txtRentID, Rent Rent, Payment NewPayment)
        //{
        //    Payment aPayment = new Payment();
        //    if (HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        aPayment.Username = HttpContext.User.Identity.Name;
        //    }
        //    aPayment.Amount = NewPayment.Amount;
        //    aPayment.Notes = NewPayment.Notes;
        //    aPayment.RentID = txtRentID;
        //    aPayment.PayTime = DateTime.Now;
            
        //    var aRent = _db.Rents.Single(r => r.RentID == txtRentID);
        //    aRent.Payments.Add(aPayment);
            
        //    aRent.TotalPayment += aPayment.Amount;
            
        //    _db.SubmitChanges();
        //    return Json("OK");
        //}

        public  ActionResult AddPayment(int rentId, int paymentAmount)
        {
            var aRent = db.Rents.Single(r => r.RentID == rentId);
            Payment payment = new Payment();
            payment.Amount = paymentAmount;
            payment.PayTime = DateTime.Now;

            aRent.Payments.Add(payment);
            aRent.TotalPayment += paymentAmount;

            db.SubmitChanges();

            return PaymentDetails(rentId,true);
        }

        public ActionResult RemovePayment(int rentId, int paymentId)
        {
            var aRent = db.Rents.Single(r => r.RentID == rentId);

            var payment = db.Payments.Single(p => p.PaymentID == paymentId);
            db.Payments.DeleteOnSubmit(payment);

            aRent.TotalPayment -= payment.Amount;
            
           

            db.SubmitChanges();

            return PaymentDetails(rentId,true);
        }



        public ActionResult PaymentDetails(int rentID, bool isEditable)
        {
            var rent = db.Rents.Single(r => r.RentID == rentID);

            PrepaidDetailViewModel model = new PrepaidDetailViewModel() { Payments= rent.Payments, IsEditable= isEditable };
            return View("_PaymentDetails", model);
        }
    }
}
