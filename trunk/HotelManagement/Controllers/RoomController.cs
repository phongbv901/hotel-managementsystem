using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using HotelBLL.Models;
using HotelDAL;
using System.Web.Configuration;

namespace HotelManagement.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        //
        // GET: /Room/
         HotelDataContext _db = new HotelDataContext(WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString);
        
        
        public ActionResult Index()
        {

            var rooms = (from room in _db.Rooms
                         where room.RoomStatus != (int) RoomStatusEnum.NotUse
                         select room).ToList();
            return View(rooms);
            
        }



        public PartialViewResult GetRoom(int id)
        {
            var room = _db.Rooms.Where(r => r.RoomID == id).Single();
            return PartialView("_Room", room);
        }

        //[HttpPost]
        //public ActionResult BookRoom(RentViewModel bookRoomView)
        //{
        //    int roomId = bookRoomView.Rent.RoomID;

        //    //Update room status
        //    var room = _db.Rooms.Where(r => r.RoomID == roomId).Single();
        //    room.RoomStatus = (int)RoomStatusEnum.InUse;

        //    var rent = bookRoomView.Rent;
        //    rent.RentStatus = (int)RentStatusEnum.InStay;

        //    _db.Rents.InsertOnSubmit(rent);
            
            
            
        //    Customer cus = bookRoomView.Customer;
        //   _db.Customers.InsertOnSubmit(cus);

           
        //    _db.SubmitChanges();


        //    //CustomerRent rentDetail = new CustomerRent();
        //    //rentDetail.CustomerID = cus.CustomerID;
        //    //rentDetail.RentID = rent.RentID;
        //    //_db.CustomerRents.InsertOnSubmit(rentDetail);
        //    if (HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        rent.CheckInPerson = HttpContext.User.Identity.Name;
        //    }
            
        //    _db.SubmitChanges();
           
        //    return PartialView("_Room",room);
        //}


        //[HttpGet]
        //public ActionResult BookRoom(int RoomID)
        //{
        //    RentViewModel model = new RentViewModel();
        //    model.Rent.RoomID = RoomID;
        //    model.Rent.Room = _db.Rooms.Where(r => r.RoomID == RoomID).Single();
        //    //model.RentTypes = _db.RentTypes.ToList();
        //    return PartialView("~/Views/Room/BookRoom.cshtml", model);
        //}

        //[HttpGet]
        //public ActionResult ShowRentInfo(int RentID)
        //{
        //    RentViewModel model = new  RentViewModel();
        //    //model.RentTypes = _db.RentTypes.ToList();
        //    var rent = _db.Rents.Where(r => r.RentID == RentID);
        //    if (rent.Count() > 0)
        //    {
        //        model.Rent = rent.First();
        //       // model.Customer = _db.Customers.Where(c => c.CustomerID == (int)model.Rent.CustomerRents.First().CustomerID).First();
        //    }
           

        //    return PartialView("~/Views/Room/BookRoom.cshtml", model);
        //}

        //[HttpPost]
        //public ActionResult UpdateRentInfo(RentViewModel bookRoomView)
        //{
        //    var rent = _db.Rents.Where(r => r.RentID == bookRoomView.Rent.RentID).FirstOrDefault();

        //    if (rent != null)
        //    {
        //        rent.CheckInDate = bookRoomView.Rent.CheckInDate;
        //        rent.RentType = bookRoomView.Rent.RentType;
        //        rent.Notes = bookRoomView.Rent.Notes;

        //        var customer = _db.Customers.Where(cus => cus.CustomerID == bookRoomView.Customer.CustomerID).First();
        //        customer.PersonID = bookRoomView.Customer.PersonID;
        //        customer.CustomerName = bookRoomView.Customer.CustomerName;
        //        //customer.BikeID = bookRoomView.Customer.BikeID;
        //        _db.SubmitChanges();


        //        return PartialView("_Room", rent.Room);
        //    }
        //    else
        //    {
        //        return Content("fail");
        //    }
           
        //}


        
        //public ActionResult ChangeRoomStatus(FormCollection form)
        //{
        //    int roomId = int.Parse(form["txtRoomId"]);
        //    int statusId = int.Parse(form["statusId"]);
        //    var room = _db.Rooms.Where(r => r.RoomID == roomId).Single();
        //    room.RoomStatus = statusId;
        //    _db.SubmitChanges();
        //    RoomView roomView = new RoomView();
        //    roomView.Room = room;
        //    roomView.RentID = 123;
        //    return PartialView("_Room", roomView);
        //}
        //public ActionResult GetCustomers(string term)
        //{
        //    var cus = _db.Customers.Where(c => c.CustomerName.Contains(term)).Select(c => new {label=c.CustomerName,PersonID= c.PersonID,BikeID=c.BikeID});
        //    return Json(cus,JsonRequestBehavior.AllowGet);
        //}
        
    
        
    }
}
