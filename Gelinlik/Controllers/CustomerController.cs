using Gelinlik.Models.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gelinlik.Controllers
{
    public class CustomerController : Controller
    {
        Context c = new Context();

        // 📌 1. Müşteri Listesi
        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Customers = c.Customers.ToList(),
                Products = c.Products.ToList(),
                Categories = c.Categorys.ToList(),
            };

            return View(model);
        }

        // 📌 2. Yeni Müşteri Ekle (GET)
        [HttpGet]
        public ActionResult NewCustomer()
        {
            var model = new HomeViewModel
            {
                Customer = new Customer { createdAt = DateTime.Now },
                Customers = c.Customers.ToList()
            };
            return View(model);
        }

        // 📌 3. Yeni Müşteri Ekle (POST)
        [HttpPost]
        public ActionResult NewCustomer(HomeViewModel model)
        {
            if (model == null || model.Customer == null)
            {
                return HttpNotFound();
            }

            if (model.Customer.createdAt == null)
            {
                model.Customer.createdAt = DateTime.Now;
            }

            c.Customers.Add(model.Customer);
            c.SaveChanges();

            // ✅ Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Müşteri Ekleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Yeni müşteri eklendi: {model.Customer.fullName} (ID: {model.Customer.id})"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Customer");
        }

        public ActionResult CustomerFind(int id)
        {
            var cf = c.Customers.Find(id);
            if (cf == null)
            {
                return HttpNotFound();
            }

            var model = new HomeViewModel
            {
                Customer = cf
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateCustomer(HomeViewModel model)
        {
            var uc = c.Customers.Find(model.Customer.id);
            if (uc == null)
            {
                return HttpNotFound();
            }

            // Güncelleme işlemi
            uc.fullName = !string.IsNullOrEmpty(model.Customer.fullName) ? model.Customer.fullName : uc.fullName;
            uc.email = !string.IsNullOrEmpty(model.Customer.email) ? model.Customer.email : uc.email;
            uc.phone = !string.IsNullOrEmpty(model.Customer.phone) ? model.Customer.phone : uc.phone;
            uc.address = !string.IsNullOrEmpty(model.Customer.address) ? model.Customer.address : uc.address;
            uc.updateAt = DateTime.Now;

            c.SaveChanges();

            // ✅ Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Müşteri Güncelleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Müşteri güncellendi: {uc.fullName} (ID: {uc.id})"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Customer");
        }

        public ActionResult RemoveCustomer(int id)
        {
            var rc = c.Customers.Find(id);
            if (rc == null)
            {
                return HttpNotFound();
            }

            c.Customers.Remove(rc);
            c.SaveChanges();

            // ✅ Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Müşteri Silme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Müşteri silindi: {rc.fullName} (ID: {rc.id})"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Customer");
        }
    }
}
