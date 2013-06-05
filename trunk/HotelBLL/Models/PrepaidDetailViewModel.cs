using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class PrepaidDetailViewModel
    {
        public IEnumerable<Payment> Payments { get; set; }
        public bool IsEditable { get; set; }
    }
}