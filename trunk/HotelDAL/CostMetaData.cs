using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace HotelDAL
{
    [MetadataType(typeof(CostMetaData))]
    public partial class Cost
    {

    }
    class CostMetaData
    {
        [Required(ErrorMessage = "Hãy điền số tiền")]
        [DataType(DataType.Currency,ErrorMessage = "Sai định dạng")]
        [Range(typeof(int),"0","10000000",ErrorMessage = "Số tiền không hợp lệ")]
        [RegularExpression(@"^[+-]?[0-9]{1,3}(?:[0-9]*(?:[.,][0-9]{2})?|(?:,[0-9]{3})*(?:\.[0-9]{2})?|(?:\.[0-9]{3})*(?:,[0-9]{2})?)$",ErrorMessage = "Số tiền không hợp lệ")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "Hãy điền nội dung chi phí")]
        [DataType(DataType.MultilineText)]
        public string CostDescription { get; set; }

        [Required(ErrorMessage = "Hãy điền ngày chi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime CostDate { get; set; }
    }
}
