using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelDAL;

namespace HotelBLL.Models
{
    public class ProductItemViewModel
    {
        public ProductItem ProductItem { get; set; }
        public int CheckedQuantity { get; set; }// So san pham o thoi diem kiem tra
       
        public int HistoryId { get; set; }
        public int Index { get; set; }
        public int SoldQuantity { get; set; }// So san pham da ban den thoi diem
        public int InventoryQuantity { get; set; }//So san pham nhap xuat kho
        public int RealisticQuantity { get; set; } //So san pham thuc te
    }
}