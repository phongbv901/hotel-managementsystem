using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelDAL;
using HotelBLL.Models;
using System.Web.Configuration;


namespace HotelManagement.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        //
        // GET: /Inventory/

        private  HotelDataContext _db = new HotelDataContext(WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString);

        public ActionResult Index()
        {
            ////Calculate all sold products
           
            var historyList = from inventoryHistory in _db.InventoryHistories
                              orderby inventoryHistory.HistoryID descending
                              select inventoryHistory;
            return View(historyList);
        }

        public ActionResult UpdateInventoryHistory(List<int> productItemList, List<int> realisticList)
       {
            InventoryHistory history = new InventoryHistory();
            history.CheckingDate = DateTime.Now;

            for (int i = 0; i < productItemList.Count; i++ )
            {
                Inventory inventory = new Inventory();
                inventory.ItemID = productItemList[i];
                inventory.Quantity = realisticList[i];
                history.Inventories.Add(inventory);
            }
            _db.InventoryHistories.InsertOnSubmit(history);
            _db.SubmitChanges();
           return Content("1");
       }

        public ActionResult ReportProductItemInventory(int HistoryId, DateTime EndTime)
        {
            SearchProductItemViewModel model = new SearchProductItemViewModel();
            InventoryHistory history = _db.InventoryHistories.Where(item => item.HistoryID == HistoryId).FirstOrDefault();
            if (history != null)
            {
                int historyId = history.HistoryID;
                DateTime historyDate = history.CheckingDate;

                var checkedProductItems = from inventory in _db.Inventories
                                          from productItem in _db.ProductItems
                                          where
                                              inventory.HistoryID == historyId &&
                                              inventory.ItemID == productItem.ItemID
                                          select new ProductItemViewModel()
                                                     {
                                                         ProductItem = productItem,
                                                         HistoryId = historyId,
                                                         CheckedQuantity = inventory.Quantity
                                                     };
                //Lay cac san pham duoc ban tu thoi diem thong ke gan nhat den hien tai
               
                var sellProductItem = from orderDetail in _db.OrderDetails
                                      from composition in _db.Compositions
                                      where orderDetail.OrderDate >= historyDate &&
                                            orderDetail.OrderDate <= EndTime &&
                                            composition.ProducID == orderDetail.ProductID
                                      select
                                          new
                                              {
                                                  composition.ItemID,
                                                  ItemQuantity = composition.Quantity,
                                                  ProductQuantity = orderDetail.Quantity
                                              };
                //Tinh tong san pham da ban
                var totalSellProductItem = from s in sellProductItem
                                           group s by s.ItemID
                                           into tmp
                                           select
                                               new
                                                   {
                                                       ItemID = tmp.Key,
                                                       SoldQuantity = tmp.Sum(t => (t.ItemQuantity*t.ProductQuantity))
                                                   };

                //Tinh tong san pham xuat nhap kho
                var totalInventoryProductItem = from productInventory in _db.ProductInventories
                                                where productInventory.ChangeDate >= historyDate &&
                                                      productInventory.ChangeDate <= EndTime
                                                group productInventory by productInventory.ItemID
                                                into tmp
                                                select
                                                    new
                                                        {
                                                            ItemID = tmp.Key,
                                                            InventoryQuantity =
                                                    tmp.Sum(t => t.Quantity*(t.IsImport ? 1 : -1))
                                                        };


                //Tao ra danh sach gui len view
                var productItemList = from c in checkedProductItems
                                      let sold =
                                          totalSellProductItem.Where(t => t.ItemID == c.ProductItem.ItemID).
                                          FirstOrDefault()
                                      let inventory =
                                          totalInventoryProductItem.Where(t => t.ItemID == c.ProductItem.ItemID).
                                          FirstOrDefault()
                                      let soldQuantity = (sold == null) ? 0 : sold.SoldQuantity
                                      let inventoryQuantity = (inventory == null) ? 0 : inventory.InventoryQuantity
                                      select new ProductItemViewModel()
                                                 {
                                                     ProductItem = c.ProductItem,
                                                     CheckedQuantity = c.CheckedQuantity,
                                                     HistoryId = c.HistoryId,
                                                     SoldQuantity = soldQuantity,
                                                     InventoryQuantity =inventoryQuantity,
                                                     RealisticQuantity = c.CheckedQuantity - soldQuantity + inventoryQuantity
                                                 };





                model.ProductItems = productItemList.ToList();
                model.StartDate = historyDate;
                model.EndDate = EndTime;

                
            }

            return PartialView("_SearchProductItemTable", model);

        }


    }
}
