using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelDAL;
using System.Web.Configuration;
using System.IO;
using HotelDAL;
using HotelBLL.Models;

namespace HotelManagement.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        HotelDataContext _db = new HotelDataContext(WebConfigurationManager.ConnectionStrings["HotelDBConnectionstring"].ConnectionString);
        public ActionResult ListProduct()
        {
            ProductViewModal model = new ProductViewModal();
            var item = _db.ProductItems.ToList();
            var category = _db.ProductCategories.ToList();
            model.item = item;
            model.category = category;
            return View(model);
        }
        // partial view for load product by category
        public ActionResult LoadProductByCategory(int index)
        {
            IEnumerable<Product> products = null;
            if (index == 99)
            {
                products = _db.Products.ToList();
            }
            else
            {
                products = _db.Products.Where(x => x.CatID == index);
            }
            return PartialView("LoadProductByCategory", products);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string ProductName, int Price, int CatID, HttpPostedFileBase PicURL)
        {
            Product product = new HotelDAL.Product();
            var filename = Path.GetFileName(PicURL.FileName);
            var path = Path.Combine(Server.MapPath("~/Content/imgs/product"), filename);
            PicURL.SaveAs(path);
            product.PicURL = filename;
            product.CatID = CatID;
            product.ProductName = ProductName;
            product.Price = Price;
            _db.Products.InsertOnSubmit(product);
            _db.SubmitChanges();
            return RedirectToAction("ListProduct");
        }

        public ActionResult Delete(int ProductID)
        {
            Product product = _db.Products.Single(e => e.ProductID == ProductID);
            {
                _db.Products.DeleteOnSubmit(product);
                _db.SubmitChanges();
            }

            return RedirectToAction("ListProduct");
        }

        public ActionResult listItem()
        {

            var item = _db.ProductItems.ToList();
            return PartialView(item);
        }
    }
}
