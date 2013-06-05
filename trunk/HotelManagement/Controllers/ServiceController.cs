using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelBLL.Models;
using HotelDAL;
using System.Web.Configuration;

namespace HotelManagement.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        HotelDataContext _db = new HotelDataContext(WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString);

        public ActionResult LoadServiceForm(int txtRentId)
        {
            var rent = _db.Rents.Single(r => r.RentID == txtRentId);
            var categories = _db.ProductCategories.ToList();
            ServiceViewModel serviceViewModel = new ServiceViewModel();
            serviceViewModel.Rent = rent;
            serviceViewModel.Categories = categories;
            return View("_ServiceForm",serviceViewModel);
        }
        public ActionResult LoadItemByCategory(int cateId, int txtRentId)
        {
            IEnumerable<Product> products = null;
            if (cateId == 0)
            {
                //Chon tat ca
                products = _db.Products.ToList();
            }
            else
            {
                products = _db.Products.Where(p => p.CatID == cateId);
            }
            MenuViewModel menuView = new MenuViewModel { Products = products, RentID = txtRentId };
            return PartialView("~/Views/Service/_MenuByCategory.cshtml", menuView);
        }

        public ActionResult AddItem(int rentId, int productId, int quantity)
        {
            var product = (from p in _db.Products
                           where p.ProductID == productId
                          select p).FirstOrDefault();
            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ProductID = productId;
            orderDetail.RentID = rentId;
            orderDetail.Quantity = quantity;
            orderDetail.OrderDate = DateTime.Now;
            orderDetail.TotalPrice = product.Price * quantity;
            _db.OrderDetails.InsertOnSubmit(orderDetail);
            _db.SubmitChanges();
            OrderDetailViewModel model = new OrderDetailViewModel()
                                             {
                                                 IsEditable = true,
                                                 OrderID = orderDetail.OrderID,
                                                 OrderDate = orderDetail.OrderDate,
                                                 PicURL = orderDetail.Product.PicURL,
                                                 Price = orderDetail.Product.Price,
                                                 ProductID = orderDetail.ProductID,
                                                 ProductName = orderDetail.Product.ProductName,
                                                 Quantity = orderDetail.Quantity,
                                                 TotalPrice = orderDetail.TotalPrice
                                             };
           
            return PartialView("~/Views/Service/_OrderItem.cshtml", model);
        }
        public ActionResult LoadItems(int rentId, bool editable=true)
        {
            var orders = _db.OrderDetails.Where(o => o.RentID == rentId);
            IEnumerable<OrderDetailViewModel> ordersDetailViewModels = null;
            if (editable)
            {
                ordersDetailViewModels = from order in orders
                                         select new OrderDetailViewModel()
                                                    {
                                                        IsEditable = true,
                                                        OrderID = order.OrderID,
                                                        OrderDate = order.OrderDate,
                                                        PicURL = order.Product.PicURL,
                                                        Price = order.Product.Price,
                                                        ProductID = order.ProductID,
                                                        ProductName = order.Product.ProductName,
                                                        Quantity = order.Quantity,
                                                        TotalPrice = order.TotalPrice
                                                    };
            }

            else
                {
                    //Group Sum
                    ordersDetailViewModels = from order in orders
                                             group order by
                                                 new
                                                     {
                                                         order.ProductID,
                                                         order.Product.ProductName,
                                                         order.Product.PicURL,
                                                         order.Product.Price
                                                     }
                                             into sumOrder
                                             let quantity = sumOrder.Sum(r => r.Quantity)
                                             let totalPrice = sumOrder.Sum(r => r.TotalPrice)
                                             select new OrderDetailViewModel
                                                        {
                                                            IsEditable = false,
                                                            //OrderID = order.OrderID,
                                                            // OrderDate = order.OrderDate,
                                                            PicURL = sumOrder.Key.PicURL,
                                                            Price = sumOrder.Key.Price,
                                                            ProductID = sumOrder.Key.ProductID,
                                                            ProductName = sumOrder.Key.ProductName,
                                                            Quantity = quantity,
                                                            TotalPrice = totalPrice
                                                        };

                }

                RentOrdersViewModel model = new RentOrdersViewModel();

                model.OrderDetailViewModels = ordersDetailViewModels;
                model.IsEditable = editable;

                return PartialView("_AllOrderItems", model);
            
        }

        public ActionResult RemoveItem(int orderId)
        {
            var order = _db.OrderDetails.Where(o => o.OrderID == orderId).FirstOrDefault();
            if (order != null)
            {
                _db.OrderDetails.DeleteOnSubmit(order);
                _db.SubmitChanges();
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

    }
}
