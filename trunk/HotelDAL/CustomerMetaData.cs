using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using  HotelDAL;
using System.ComponentModel.DataAnnotations;

namespace HotelDAL
{
    [MetadataType(typeof(CustomerMetaData))]
    public  partial class  Customer
    {
         
    }
    class CustomerMetaData
    {

        
        [Display(Name = "Số CMND")]
        public string PersonID { get; set; }


  
        [Display(Name = "Tên khách thuê")]
        public string CustomerName { get; set; }

        [Display(Name = "Số xe")]
        public string BikeID { get; set; }
        

        
    }
}
