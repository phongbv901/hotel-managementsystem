﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class ServiceViewModel
    {
        public Rent  Rent { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
    }
}