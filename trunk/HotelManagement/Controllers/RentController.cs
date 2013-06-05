using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelBLL.Models;
using HotelDAL;
using HotelFee;
using HotelBLL.Models.RentModels;
using System.Web.Configuration;
using HotelOfflineBLL.BusinessLogic;

namespace HotelManagement.Controllers
{
    [Authorize]
    public class RentController : Controller
    {
        //
        // GET: /Checkout/
        private string connectionString;
        private RentBusinessLogic rentBusiness;

        public  RentController()
        {
            connectionString = WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString;
            rentBusiness = new RentBusinessLogic(connectionString);
        }
        public ActionResult Index(int rentId)
        {
            Rent rent = rentBusiness.GetRentById(rentId);
            //_db.SubmitChanges();
            return PartialView("RentDetail", rent);

        }

        public ActionResult ViewRentReadonly(int rentId)
        {
            Rent rent = rentBusiness.GetRentById(rentId);
            return PartialView("RentDetailReadOnly", rent);

        }



        #region Rent Booking

        public ActionResult ShowBookingForm(int roomId, string roomName)
        {
            Room room = new Room() {RoomID = roomId, RoomName = roomName};
            return PartialView("RentBooking", room);
        }

        public ActionResult BookRoom(int roomId,string invoiceId, DateTime checkInDate,int rentType,string bikeId,string notes)
        {
                var rent = new Rent();
                rent.RoomID = roomId;
                rent.InvoiceID = invoiceId;
                rent.RentStatus = (int)RentStatusEnum.InStay;
                rent.CheckInPerson = HttpContext.User.Identity.Name;
                rent.CheckInDate = checkInDate;
                rent.RentType = rentType;
                rent.BikeID = bikeId;
                rent.Notes = notes;
                
                
                int rentId = rentBusiness.BookRoom(rent);
                return Content(rentId.ToString());

        }

        #endregion

#region RENT UPDATING - CHECKOUT

        public ActionResult CalculateRentFee(int rentId, int rentType, DateTime checkInDate, DateTime checkOutDate)
        {
            RentBusinessLogic.FeeDescription fee = rentBusiness.CalculateRentFee(rentId, rentType, checkInDate,
                                                                                 checkOutDate);
            return Json(new { rentFee = fee.RentFee, description = fee.RentDescription }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRentInfo(int rentId, string invoiceId, DateTime checkInDate, int rentType, string bikeId, string notes)
        {
           bool rs = rentBusiness.UpdateRentInfo(rentId, invoiceId, checkInDate, rentType, bikeId, notes);
           return Content(rs?"1":"0");
           

        }

        /// <summary>
        /// Handle event when user checkout rent.
        /// </summary>
        /// <param name="rentId"></param>
        /// <param name="checkInDate"></param>
        /// <param name="checkOutDate"></param>
        /// <param name="rentType"></param>
        /// <param name="isPayAll"></param>
        /// <returns></returns>
        public ActionResult CheckOut(int rentId, DateTime checkInDate, DateTime checkOutDate, int rentType, bool isPayAll)
        {
            string username = HttpContext.User.Identity.Name;
            bool rs =  rentBusiness.CheckOut(rentId, checkInDate, checkOutDate, rentType, isPayAll, username);
            return Content(rs ? "1" : "0");
        }


#endregion

        #region Rent Customer
        public ActionResult GetRentCustomers(int rentId)
        {
            var customers = rentBusiness.GetRentCustomers(rentId);
            return PartialView("_RentCustomer", customers);
        }

        public ActionResult AddRentCustomers(int rentId,string personId,string customerName,string address)
        {
            var customer = rentBusiness.AddRentCustomers(rentId, personId, customerName, address);
            return PartialView("_RentCustomerItem", customer);
        }

        public ActionResult RemoveRentCustomers(int customerId)
        {
            bool rs = rentBusiness.RemoveRentCustomers(customerId);
            return Content(rs ? "1" : "0");

        }

        #endregion

#region Rent Payment
        public ActionResult GetRentPayments(int rentId)
        {
            var payments = rentBusiness.GetRentPayments(rentId);
            return View("_RentPrepaid", payments);
        }

        public ActionResult AddPayment(int rentId, int paymentAmount, string notes)
        {
            Payment payment = new Payment();
            payment.RentID = rentId;
            payment.Amount = paymentAmount;
            payment.PayTime = DateTime.Now;
            payment.Notes = notes;
            payment.Username = HttpContext.User.Identity.Name;

            payment =rentBusiness.AddRentPayment(payment);
            return PartialView("_RentPrepaidItem",payment);
        }

        public ActionResult RemovePayment(int paymentId)
        {
            bool rs = rentBusiness.RemoveRentPayment(paymentId);

            return Content(rs?"1":"0");
        }


#endregion

        #region Rent Fee Change
        public ActionResult GetRentFeeChange(int rentId, bool  isAddition)
        {
            var feeChanges = rentBusiness.GetRentFeeChange(rentId,isAddition);
            if (isAddition)
            {
                return View("_RentFeeChangeAddition", feeChanges);
            }
            else
            {
                return View("_RentFeeChangeDiscount", feeChanges);
            }
        }

        public ActionResult AddFeeChange(int rentId, int feeChangeAmount, string notes, bool isAddition)
        {
            RentFeeChange feeChange = new RentFeeChange();
            feeChange.RentID = rentId;
            feeChange.Amount = feeChangeAmount;
            feeChange.AddTime = DateTime.Now;
            feeChange.Notes = notes;
            feeChange.Username = HttpContext.User.Identity.Name;
            feeChange.IsAddition = isAddition;

            feeChange = rentBusiness.AddRentFeeChange(feeChange);
            return PartialView("_RentFeeChangeItem", feeChange);
        }

        public ActionResult RemoveFeeChange(int feeChangeId)
        {
            bool rs = rentBusiness.RemoveRentPayment(feeChangeId);

            return Content(rs ? "1" : "0");
        }


        #endregion

#region Rent Service
        public ActionResult GetRentService(int rentId)
        {
         
            ViewBag.RentId= rentId;
            ViewBag.ServiceCategory = rentBusiness.GetRentServiceCategory(); 
            return View("_RentService");
        }

#endregion


    }
}
