using Gelinlik.Models.Class;
using System.Web.Mvc;
using System.Linq;
using Antlr.Runtime.Misc;
using System;

namespace Gelinlik.Controllers
{
    public class ProductsController : Controller
    {
        Context c = new Context();

        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Products = c.Products.ToList(),
                Categories = c.Categorys.ToList(),
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult NewProducts()
        {
            var model = new HomeViewModel
            {
                Products = c.Products.ToList(),
                Categories = c.Categorys.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult NewProducts(HomeViewModel model)
        {
            if (model == null || model.Product == null)
            {
                model = new HomeViewModel { Categories = c.Categorys.ToList() };
                return View(model);
            }

            bool isAvailableValue = Request.Form["Product.isAvailable"] == "true";
            model.Product.isAvailable = isAvailableValue;

            c.Products.Add(model.Product);
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Ürün Ekleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Yeni ürün eklendi. Ürün Adı: {model.Product.name}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public ActionResult ProductsFind(int id)
        {
            var product = c.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var model = new HomeViewModel
            {
                Product = product,
                Categories = c.Categorys.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateProducts(HomeViewModel model)
        {
            var up = c.Products.Find(model.Product.id);
            if (up == null)
            {
                return HttpNotFound();
            }

            up.name = !string.IsNullOrEmpty(model.Product.name) ? model.Product.name : up.name;
            up.description = !string.IsNullOrEmpty(model.Product.description) ? model.Product.description : up.description;
            up.price = model.Product.price > 0 ? model.Product.price : up.price;
            up.rentalPrice = model.Product.rentalPrice > 0 ? model.Product.rentalPrice : up.rentalPrice;
            up.CategoryId = model.Product.CategoryId != 0 ? model.Product.CategoryId : up.CategoryId;
            up.stock = model.Product.stock != 0 ? model.Product.stock : up.stock;

            if (!string.IsNullOrEmpty(Request.Form["Product.isAvailable"]))
            {
                up.isAvailable = Request.Form["Product.isAvailable"] == "true";
            }

            up.updateAt = DateTime.Now;
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Ürün Güncelleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Ürün güncellendi. Ürün Adı: {up.name}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public ActionResult RemoveProducts(int id)
        {
            var product = c.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var rentals = c.Rentals.Where(r => r.ProductId == id).ToList();

            if (rentals.Any(r => !r.IsReturned))
            {
                TempData["ErrorMessage"] = "Bu ürüne ait iade edilmemiş kiralama kayıtları mevcut! Önce kiralamaları iade edin.";
                return RedirectToAction("Index", "Products");
            }

            foreach (var rental in rentals)
            {
                c.Rentals.Remove(rental);
            }

            c.Products.Remove(product);
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Ürün Silme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Ürün silindi. Ürün Adı: {product.name}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            TempData["SuccessMessage"] = "Ürün ve ilgili kiralama kayıtları başarıyla silindi.";
            return RedirectToAction("Index", "Products");
        }

        public ActionResult ConfirmRemoveProduct(int id)
        {
            var product = c.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProductId = id;
            return View();
        }
    }
}
