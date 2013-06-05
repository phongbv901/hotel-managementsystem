using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HotelFee;
using HotelDAL;
namespace HotelManagement.Models
{
    public class CheckoutViewModel
    {
        public Rent Rent { get; set; }

        public bool IsPayAll { get; set; }

       // public IEnumerable<RentType> RentTypes { get; set; }


        public PriceSummaryModel PriceSummaryModel { get; set; }

        public bool IsEditMode { get; set; }
             

    }
}