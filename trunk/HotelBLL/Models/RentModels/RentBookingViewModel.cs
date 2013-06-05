using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelDAL;

namespace HotelBLL.Models.RentModels
{
    public class RentBookingViewModel
    {
        public Room Room { get; set; }
        public Rent Rent { get; set; }
    }
}
