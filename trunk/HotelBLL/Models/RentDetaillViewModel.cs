using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HotelFee;
using HotelDAL;
namespace HotelBLL.Models
{
    public class RentDetailViewModel
    {
        public Rent Rent { get; set; }

        public bool IsNew { get; set; }
             

    }
}