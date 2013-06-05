using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;
using HotelDAL;
namespace HotelBLL.Models
{
    public class RentViewModel
    {
        public Customer Customer { get; set; }
        public Rent Rent { get; set; }
        //public IEnumerable<RentType> RentTypes { get; set; }

        public  RentViewModel()
        {
            Customer = new Customer();
            Rent = new Rent();
        }
    }

    
    
   
}