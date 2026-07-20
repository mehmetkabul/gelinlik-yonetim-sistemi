using Gelinlik.Models.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gelinlik.Controllers
{
    public class SalesController : Controller
    {
        // GET: Sales
        Context c = new Context();
        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Customers = c.Customers.ToList(),
                Products = c.Products.ToList(),
                Categories = c.Categorys.ToList(),
                Sales = c.Sales.Include("Customer").Include("Product").ToList(),
            };


            return View(model);
        }

        [HttpGet]
        public ActionResult NewSale()
        {
            var model = new HomeViewModel
            {
                Customers = c.Customers.ToList(),
                Categories = c.Categorys.ToList(),
                Products = new List<Products>(), // Başlangıçta boş
                Sale = new Sales()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult NewSale(HomeViewModel model)
        {
            if (model == null || model.Sale == null)
            {
                return HttpNotFound();
            }

            var product = c.Products.Find(model.Sale.ProductId);

            // Ürün mevcut mu ve yeterli stok var mı kontrol et
            if (product == null || product.stock < model.Sale.Quantity)
            {
                ViewBag.StockError = "Seçilen ürün için yeterli stok yok!";
                model.Customers = c.Customers.ToList();
                model.Categories = c.Categorys.ToList();
                model.Products = new List<Products>();
                return View(model);
            }

            // Satışı kaydet
            model.Sale.SaleDate = DateTime.Now;
            model.Sale.TotalPrice = product.price * model.Sale.Quantity;

            c.Sales.Add(model.Sale);

            // Stoktan düş
            product.stock -= model.Sale.Quantity;
            if (product.stock == 0)
            {
                product.isAvailable = false;  // Stok bitti, artık satışta gözükmesin
            }

            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var customer = c.Customers.Find(model.Sale.CustomerId);

            var activity = new Activity
            {
                Action = "Satış",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Satış Yapıldı: Müşteri: {customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {product?.name ?? "Ürün Bulunamadı"}, Miktar: {model.Sale.Quantity}, Toplam Fiyat: {model.Sale.TotalPrice:C}"
            };

            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Sales");
        }


        public JsonResult GetProductsByCategory(int categoryId)
        {
            var products = c.Products
                            .Where(p => p.CategoryId == categoryId && p.isAvailable)
                            .Select(p => new { p.id, p.name, p.price })
                            .ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
        }
    }
}