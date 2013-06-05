using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelDAL;
using HotelBLL.Models;
using System.Web.Security;
using System.Web.Configuration;

namespace HotelManagement.Controllers
{
    [Authorize]
    public class CostController : Controller
    {

        HotelDataContext _db = new HotelDataContext(WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString);

        public ActionResult Index()
        {
            var categories = _db.CostCategories.ToList();

            return View(categories);
        }

        public ActionResult SearchCostByStatus(int status, int pageIndex, int pageSize)
        {
            var count = _db.Costs.Where(c => c.CostStatus == status).Count();
            int totalPage = (count / pageSize) + (count % pageSize == 0 ? 0 : 1);
            var costs = _db.Costs.Where(r => r.CostStatus == status).OrderByDescending(r => r.CostID).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var model = new SearchCostTableViewModel();
            model.Costs = costs;
            model.TotalPage = totalPage;
            model.CurrentPageIndex = pageIndex;
            model.Status = status;
            model.IsAdmin = Roles.GetRolesForUser().Contains("admin");
            return PartialView("_SearchCostTable", model);
        }


        public ActionResult ChangeCostStatus(int costId, int fromStatus, int toStatus, int index)
        {
            var item = _db.Costs.Where(r => r.CostID == costId).FirstOrDefault();
            if ((item != null) && CheckValidTransition((CostStatusEnum)fromStatus, (CostStatusEnum)toStatus))
            {

                if (toStatus == (int)CostStatusEnum.Deleted)
                {
                    _db.Costs.DeleteOnSubmit(item);

                }
                else
                {
                    item.CostStatus = toStatus;
                }

                _db.SubmitChanges();
            }

            if (toStatus != (int)CostStatusEnum.Deleted)
            {
                CostItemViewModel model = new CostItemViewModel();
                model.Index = index;
                model.Cost = item;
                return PartialView("_CostItem", model);
            }

            return Content("");
        }


        private bool CheckValidTransition(CostStatusEnum fromStatus, CostStatusEnum toStatus)
        {
            if (fromStatus == toStatus)
                return false;

            bool isAdmin = Roles.GetRolesForUser().Contains("admin");

            switch (toStatus)
            {
                case CostStatusEnum.New:
                    return false;

                case CostStatusEnum.Approved:
                    if (isAdmin && fromStatus == CostStatusEnum.New)
                        return true;
                    else
                        return false;

                case CostStatusEnum.WaitDisabled:
                    if (fromStatus == CostStatusEnum.New || (fromStatus == CostStatusEnum.Approved && isAdmin))
                        return true;
                    else
                        return false;
                case CostStatusEnum.Disabled:
                    if (isAdmin && fromStatus == CostStatusEnum.WaitDisabled)
                        return true;
                    else
                        return false;
                case CostStatusEnum.Deleted:
                    if (isAdmin && fromStatus == CostStatusEnum.Disabled)
                        return true;
                    else
                        return false;

            }
            return false;

        }















        // HotelDataContext _db = new HotelDataContext(WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString);
        ////
        //// GET: /Cost/


        public ActionResult ShowAddCostModal()
        {
            CostViewModel costViewModel = new CostViewModel();
            costViewModel.Cost = new Cost();
            costViewModel.Cost.CostDate = DateTime.Now;
            var categories = _db.CostCategories.ToList();
            costViewModel.CostCategories = categories;
            return PartialView("_AddCostModal", costViewModel);
        }
        //public ActionResult ShowUpdateCostModal(int CostID, int Index)
        //{
        //    CostViewModel costViewModel = new CostViewModel();
        //    costViewModel.Cost = _db.Costs.Where(c => c.CostID == CostID).Single();
        //    costViewModel.CostCategories = _db.CostCategories.ToList();
        //    costViewModel.Index = Index;
        //    return PartialView("_UpdateCostModal", costViewModel);
        //}

        public ActionResult AddCost(CostViewModel model)
        {
            Cost cost = model.Cost;

            cost.CostStatus = (int)CostStatusEnum.New;
            cost.LoggedPerson = HttpContext.User.Identity.Name;
            if (cost.CostDescription == null)
            {
                cost.CostDescription = "";
            }
            _db.Costs.InsertOnSubmit(cost);

            _db.SubmitChanges();
            return Content("1");
        }


        //public ActionResult UpdateCost(CostViewModel costViewModel)
        //{
        //    Cost cost = _db.Costs.Where(c => c.CostID == costViewModel.Cost.CostID).Single();
        //    cost.Amount = costViewModel.Cost.Amount;
        //    cost.CatID = costViewModel.Cost.CatID;
        //    cost.CostDate = costViewModel.Cost.CostDate;
        //    cost.CostDescription = costViewModel.Cost.CostDescription;
        //    cost.LoggedPerson = HttpContext.User.Identity.Name;
            
        //    _db.SubmitChanges();
        //    return RedirectToAction("CostItem", new { CostID = cost.CostID, Index = costViewModel.Index });
        //}
        //public ActionResult SearchCostByPeriod(DateTime StartTime, DateTime EndTime, string CostCategoryID)
        //{
        //    int catID = 0; // Tat ca Danh muc
        //    if (CostCategoryID != "")
        //    {
        //        catID = int.Parse(CostCategoryID);
        //    }

        //    var items =
        //        _db.Costs.Where(c => (c.CostStatus != (int)CostStatusEnum.Disabled) && (c.CostDate >= StartTime) && (c.CostDate <= EndTime) && (c.CatID == catID || catID == 0));

        //    SearchCostTableViewModel model = new SearchCostTableViewModel();
        //    model.Costs = items;
        //    model.StartDate = StartTime;
        //    model.EndDate = EndTime;

        //    return PartialView("_SearchCostTable", model);   
        //}
        //public ActionResult SearchCostByCertainDate(int Year, int Month, int Day, string CostCategoryID)
        //{
        //    int catID = 0; // Tat ca Danh muc
        //    if (CostCategoryID != "")
        //    {
        //        catID = int.Parse(CostCategoryID);
        //    }
        //    DateTime StartTime;
        //    DateTime EndTime;
        //    if (Month == 0)
        //    {
        //        StartTime = new DateTime(Year,1,1);
        //        EndTime = new DateTime(Year,12,31);
        //    }
        //    else if (Day == 0)
        //    {
        //        StartTime = new DateTime(Year,Month,1);
        //        EndTime = new DateTime(Year,Month,DateTime.DaysInMonth(Year,Month));
        //    }
        //    else
        //    {
        //        StartTime = new DateTime(Year, Month, Day, 0, 0, 0);
        //        EndTime = new DateTime(Year, Month, Day, 23,59,59);
        //    }
        //    var items =
        //        _db.Costs.Where(
        //            c =>
        //            (c.CostStatus != (int)CostStatusEnum.Disabled) && (c.CostDate >= StartTime) && (c.CostDate <= EndTime) &&
        //            (c.CatID == catID || catID == 0));

        //    SearchCostTableViewModel model = new SearchCostTableViewModel();
        //    model.Costs = items;
        //    model.StartDate = StartTime;
        //    model.EndDate = EndTime;

        //    return PartialView("_SearchCostTable", model);   
        //}

        //public ActionResult RemoveCost(int CostID, int Index)
        //{
        //    var cost = _db.Costs.Where(c => c.CostID == CostID).Single();
        //    cost.CostStatus = (int)CostStatusEnum.Disabled;
        //    _db.SubmitChanges();
        //    return Content("OK");

        //}
        //public ActionResult ApproveCost(int CostID, int Index)
        //{
        //    var cost = _db.Costs.Where(c => c.CostID == CostID).Single();
        //    cost.CostStatus = (int)CostStatusEnum.Approved;
        //    cost.ApprovedPerson = HttpContext.User.Identity.Name;
        //    _db.SubmitChanges();
        //    return RedirectToAction("CostItem", new { CostID = CostID, Index = Index });

        //}
        //public ActionResult CostItem(int CostID, int Index)
        //{
        //    var costItem = new CostItemViewModel();
        //    costItem.Cost = _db.Costs.Where(c => c.CostID == CostID).Single();
        //    costItem.Index = Index;
        //    return PartialView("_CostItem", costItem);
        //}
        //public ActionResult GetDayComboBox(int year, int month)
        //{
        //    int days = DateTime.DaysInMonth(year, month);
        //    ViewBag.Days = days;
        //    return PartialView("_DayComboBox");
        //}
    }
}
