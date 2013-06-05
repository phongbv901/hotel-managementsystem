using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class RoomWithCurrentRentViewModel
    {
        public Room Room { get; set; }
        public int CurrentRentId { get; set; }
    }
}