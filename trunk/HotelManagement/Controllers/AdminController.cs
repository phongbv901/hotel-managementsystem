using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelDAL;
using HotelBLL.Models;
using HotelManagement.Util;
using System.IO;
using System.Web.Security;
using System.Web.Configuration;

namespace HotelManagement.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
         HotelDataContext _db = new HotelDataContext(WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString);
        public ActionResult Index()
        {
            return View();

        }

       
        public ActionResult SearchRentByStatus(int status, int pageIndex, int pageSize)
        {
            var count = _db.Rents.Where(r => r.RentStatus == status).Count();
            int totalPage = (count / pageSize) + (count % pageSize == 0 ? 0 : 1);
            var model = new SearchRentTableViewModel();

            IEnumerable<Rent> rents;
            if (status == (int)RentStatusEnum.InStay)
            {
                rents = _db.Rents.Where(r => r.RentStatus == status).OrderBy(r =>r.RoomID).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                rents = _db.Rents.Where(r => r.RentStatus == status).OrderByDescending(r => r.CheckOutDate).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            
            model.Rents = rents;
            model.TotalPage = totalPage;
            model.CurrentPageIndex = pageIndex;
            model.Status = status;
            model.IsAdmin = Roles.GetRolesForUser().Contains("admin");
            return PartialView("_SearchRentTable", model);
        }

        public  ActionResult ApproveAllRents()
        {
            var item = _db.Rents.Where(r => r.RentStatus == (int) RentStatusEnum.Paid);
            foreach (var rent in item)
            {
                rent.RentStatus = (int) RentStatusEnum.Approved;
            }
            _db.SubmitChanges();
            return Content("1");
        }


        public ActionResult ExportToExcel(DateTime StartTime, DateTime EndTime)
        {
            List<string> headerStrings = new List<string> { "Tên phòng", "Loại thuê", "Thời điểm vào", "Thời điểm trả", "Tổng tiền", "Tổng thu", "Còn lại", "Trạng thái" };
            var item = from r in _db.Rents
                       where ((r.RentStatus != ((int)RentStatusEnum.Disabled)) &&
                              ((r.RentStatus == (int)(RentStatusEnum.InStay)) ||
                               ((r.CheckOutDate >= StartTime) && (r.CheckOutDate <= EndTime))))
                       select new RentItem
                       {
                           RoomName = r.Room.RoomName,
                           CheckInDate = r.CheckInDate,
                           CheckOutDate = r.CheckOutDate,
                           TotalMoney = r.RentFee + r.OrderFee,
                           TotalPayment = r.TotalPayment,
                           ContainMoney = r.RentFee + r.OrderFee - r.TotalPayment,
                           RentStatus = r.RentStatus == (int)RentStatusEnum.Approved ? "Đã duyệt" :
                                        r.RentStatus == (int)RentStatusEnum.InStay ? "Đang ở" :
                                        r.RentStatus == (int)RentStatusEnum.Paid ? "Chờ duyệt" : "Hủy"
                           ,
                           RentType = r.RentType == (int)RentTypeEnum.Day ? "Thuê ngày" :
                               r.RentType == (int)RentTypeEnum.Hour ? "Thuê giờ" : "Qua đêm"
                       };
            string fileName = string.Format("BaoCao_{0}_{1}.xlsx", StartTime.ToShortDateString(), EndTime.ToShortDateString());
            ExcelFacade excelFacade = new ExcelFacade();
            MemoryStream stream = excelFacade.CreateExcelStream<RentItem>(fileName, item.ToList(), "Sheet1", headerStrings);
            //byte[] content = new byte[stream.Length];
            //stream.Read(content, 0, (int)stream.Length);

            byte[] content = stream.ToArray();

            return new BinaryFileResult()
            {
                FileName = fileName,
                Content = content
            };
        }

        public ActionResult ChangeRentStatus(int RentID, int fromStatus, int toStatus, int index)
        {
            var item = _db.Rents.Where(r => r.RentID == RentID && r.RentStatus == fromStatus).FirstOrDefault();
            if ((item != null)&& CheckValidTransition((RentStatusEnum)fromStatus,(RentStatusEnum)toStatus))
            {
               
                if (toStatus == (int)RentStatusEnum.DeletePermanent)
                {
                    _db.Rents.DeleteOnSubmit(item);

                }
                else
                {
                     item.RentStatus = toStatus;
                }


                // Neu dang o trang thai InStay chuyển qua trang thai khac se phai cap nhat lai trang thai cua phong
                if (fromStatus == (int)RentStatusEnum.InStay)
                {
                    item.Room.RoomStatus = (int) RoomStatusEnum.Ready;
                }

                _db.SubmitChanges();
            }

           




            if (toStatus != (int)RentStatusEnum.DeletePermanent)
            {
                RentItemViewModel model = new RentItemViewModel();
                model.Index = index;
                model.Rent = item;
                return PartialView("_RentItem", model);
            }

            return Content("");
        }


        private bool CheckValidTransition(RentStatusEnum fromStatus, RentStatusEnum toStatus)
        {
            if (fromStatus == toStatus)
                return false;

            bool isAdmin = Roles.GetRolesForUser().Contains("admin");

            switch (toStatus)
            {
                case RentStatusEnum.InStay:
                    return false;
                case RentStatusEnum.Paid:
                    if (fromStatus == RentStatusEnum.InStay)
                        return true;
                    else
                        return false;
                case RentStatusEnum.Approved:
                    if (isAdmin && fromStatus == RentStatusEnum.Paid)
                        return true;
                    else
                        return false;

                case RentStatusEnum.WaitDisabled:
                    if (fromStatus == RentStatusEnum.InStay || fromStatus == RentStatusEnum.Paid || (fromStatus == RentStatusEnum.Approved && isAdmin))
                        return true;
                    else
                        return false;
                case RentStatusEnum.Disabled:
                    if (isAdmin && fromStatus == RentStatusEnum.WaitDisabled)
                        return true;
                    else
                        return false;
                case RentStatusEnum.DeletePermanent:
                    if (isAdmin && fromStatus == RentStatusEnum.Disabled)
                        return true;
                    else
                        return false;

            }
            return false;

        }

    }
}
