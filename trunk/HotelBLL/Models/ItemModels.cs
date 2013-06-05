using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelBLL.Models
{
    public partial class ItemModels
    {
        public string itemName { get; set; }
        public string unit { get; set; }
    }

    public partial class ItemModels {
        List<ItemModels> partialModel { get; set; }
    }
}
