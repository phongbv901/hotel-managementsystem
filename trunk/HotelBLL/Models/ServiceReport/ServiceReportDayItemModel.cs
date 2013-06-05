using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models.ServiceReport
{
    public class ServiceReportItemModel
    {
        public Product Product { get; set; }

        public List<ProductQuantityInDay> QuantityInDays { get; set; } 

        public class ProductQuantityInDay
        {
            public DateTime Date { get; set; }
            public int Quantity { get; set; }
        }
      
    }

   
}