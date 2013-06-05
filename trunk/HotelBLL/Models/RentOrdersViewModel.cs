using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class RentOrdersViewModel
    {
        public IEnumerable<OrderDetailViewModel> OrderDetailViewModels { get; set; }
        public bool IsEditable { get; set; }

        public int TotalOrderFee
        {
            get { 
                int totalFee = 0;
                foreach (var orderDetailViewModel in OrderDetailViewModels)
                {
                    totalFee += orderDetailViewModel.TotalPrice;
                }
                return totalFee;
            }
        }

    }
}