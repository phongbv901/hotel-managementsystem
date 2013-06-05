using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace HotelDAL
{
    [MetadataType(typeof(InventoryHistoryMetaData))]
    public partial class InventoryHistory
    {

    }
    class InventoryHistoryMetaData
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime CheckingDate { get; set; }
    }
}
