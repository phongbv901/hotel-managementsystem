using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class OrderDetailViewModel
    {
        public int OrderID { get; set; }

        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public string PicURL { get; set; }

        public int TotalPrice { get; set; }

        public int Price { get; set; }
        public int Quantity { get; set; }

        public DateTime OrderDate { get; set; }

        
        public bool IsEditable { get; set; }


      

    }
}