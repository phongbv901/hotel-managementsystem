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
    public class ProductImportController : Controller
    {

        HotelDataContext _db = new HotelDataContext(WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString);
        //
        // GET: /Cost/

        public ActionResult Index()
        {
            return View();
        }



        public ActionResult SearchProductImport(DateTime StartTime, DateTime EndTime)
        {
            var item =
                 _db.ProductInventories.Where(p=>(p.ChangeDate >=StartTime && p.ChangeDate <= EndTime)) ;
            SearchProductImportViewModel model = new SearchProductImportViewModel();
            model.ProductImports = item;
            model.StartDate = StartTime;
            model.EndDate = EndTime;
            return PartialView("_SearchProductImportTable", model);
        }

























        public ActionResult ShowAddCostModal()
        {
            CostViewModel costViewModel = new CostViewModel();
            costViewModel.Cost = new Cost();
            costViewModel.Cost.CostDate = DateTime.Now;
            var categories = _db.CostCategories.ToList();
            costViewModel.CostCategories = categories;
            return PartialView("_AddCostModal", costViewModel);
        }
        public ActionResult ShowUpdateCostModal(int CostID, int Index)
        {
            CostViewModel costViewModel = new CostViewModel();
            costViewModel.Cost = _db.Costs.Where(c => c.CostID == CostID).Single();
            costViewModel.CostCategories = _db.CostCategories.ToList();
            costViewModel.Index = Index;
            return PartialView("_UpdateCostModal", costViewModel);
        }
        public ActionResult AddCost(CostViewModel costViewModel)
        {
            Cost cost = costViewModel.Cost;
            cost.CostStatus = (int)CostStatusEnum.New;
            cost.LoggedPerson = HttpContext.User.Identity.Name;
            if (cost.CostDescription == null)
            {
                cost.CostDescription = "";
            }
            _db.Costs.InsertOnSubmit(cost);

            _db.SubmitChanges();
            return RedirectToAction("CostItem", new { CostID = cost.CostID, Index = 0 });
        }
        public ActionResult UpdateCost(CostViewModel costViewModel)
        {
            Cost cost = _db.Costs.Where(c => c.CostID == costViewModel.Cost.CostID).Single();
            cost.Amount = costViewModel.Cost.Amount;
            cost.CatID = costViewModel.Cost.CatID;
            cost.CostDate = costViewModel.Cost.CostDate;
            cost.CostDescription = costViewModel.Cost.CostDescription;
            cost.LoggedPerson = HttpContext.User.Identity.Name;
            
            _db.SubmitChanges();
            return RedirectToAction("CostItem", new { CostID = cost.CostID, Index = costViewModel.Index });
        }
       
       
        public ActionResult RemoveCost(int CostID, int Index)
        {
            var cost = _db.Costs.Where(c => c.CostID == CostID).Single();
            cost.CostStatus = (int)CostStatusEnum.Disabled;
            _db.SubmitChanges();
            return Content("1");

        }
        public ActionResult ApproveCost(int CostID, int Index)
        {
            var cost = _db.Costs.Where(c => c.CostID == CostID).Single();
            cost.CostStatus = (int)CostStatusEnum.Approved;
            cost.ApprovedPerson = HttpContext.User.Identity.Name;
            _db.SubmitChanges();
            return RedirectToAction("CostItem", new { CostID = CostID, Index = Index });

        }
        public ActionResult CostItem(int CostID, int Index)
        {
            var costItem = new CostItemViewModel();
            costItem.Cost = _db.Costs.Where(c => c.CostID == CostID).Single();
            costItem.Index = Index;
            costItem.IsAdmin = Roles.GetRolesForUser().Contains("admin");
            return PartialView("_CostItem", costItem);
        }
        public ActionResult GetDayComboBox(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            ViewBag.Days = days;
            return PartialView("_DayComboBox");
        }
    }
}
