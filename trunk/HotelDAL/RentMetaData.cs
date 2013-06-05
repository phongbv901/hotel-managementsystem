using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HotelDAL
{
    [MetadataType(typeof(RentMetaData))]
    public  partial class Rent
    {
        
    }


    public class RentMetaData
    {
        [Required(ErrorMessage = "Vui lòng mã số phiếu ")]
        [Display(Name = "Mã số phiếu")]
        public string InvoiceID { get; set; }
        
        
        [Display(Name = "Thời điểm nhận phòng")]
        [DisplayFormat(ApplyFormatInEditMode = true,DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DataType(DataType.DateTime)]
        public DateTime? CheckInDate{get; set;}

        [Display(Name = "Thời điểm trả phòng")]
        [DisplayFormat(ApplyFormatInEditMode = true,DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        [DataType(DataType.DateTime)]
        public DateTime? CheckOutDate{get; set;}

        [Display(Name = "Tiền thuê phòng")]
        public int RentFee{get; set;}

        [Display(Name = "Tiền dịch vụ")]
        public int OrderFee{get; set;}

        [Display(Name = "Tổng phí")]
        public int TotalPayment{get; set;}


        [Display(Name = "Ghi chú")]
        public string Notes{get; set;}

        [Display(Name = "Cách tính phí")]
        public string FeeDescription{get; set;}

        [Display(Name = "Nhân viên giao phòng")]
        public string CheckInPerson{get; set;}

        [Display(Name = "Nhân viên thanh toán")]
        public string CheckOutPerson{get; set;}

        [Display(Name = "Người duyệt")]
        public string ApprovePerson{get; set;}

        [Display(Name = "Hình thức thuê")]
        public int RentType { get; set; }

       
    }
}
