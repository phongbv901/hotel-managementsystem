using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelManagement.Util
{
    public class RentItem
    {
        public string RoomName { get; set; }
        public string RentType { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int TotalMoney { get; set; }
        public int TotalPayment { get; set; }
        public int ContainMoney { get; set; }
        public string RentStatus { get; set; }
    }
}