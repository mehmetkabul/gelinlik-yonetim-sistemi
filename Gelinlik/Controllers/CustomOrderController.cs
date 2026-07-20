using Gelinlik.Models.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gelinlik.Controllers
{
    public class CustomOrderController : Controller
    {
        Context c = new Context();

        public ActionResult Index()
        {
            var model = c.CustomOrders.ToList();
            var pendingOrders = c.CustomOrders.Where(x => x.Status == "Onay Bekliyor").ToList();
            var allOrders = c.CustomOrders.ToList();

            ViewBag.PendingOrders = pendingOrders;
            ViewBag.AllOrders = allOrders;

            return View(model);
        }

        [HttpGet]
        public ActionResult NewCustomOrder()
        {
            ViewBag.Customers = new SelectList(c.Customers.ToList(), "id", "fullName");
            ViewBag.Categories = new SelectList(c.Categorys.ToList(), "categoryName", "categoryName");
            return View();
        }

        [HttpPost]
        public ActionResult NewCustomOrder(CustomOrder order)
        {
            if (order.CustomerId == 0 || string.IsNullOrEmpty(order.ProductType) || order.Price <= 0 || order.DeliveryDate == default(DateTime) || order.RequestDate == default(DateTime))
            {
                ViewBag.Error = "Lütfen tüm zorunlu alanları doldurun.";
                ViewBag.Customers = new SelectList(c.Customers.ToList(), "id", "fullName");
                ViewBag.Categories = new SelectList(c.Categorys.ToList(), "categoryName", "categoryName");
                return View(order);
            }

            var selectedCustomer = c.Customers.FirstOrDefault(x => x.id == order.CustomerId);
            if (selectedCustomer != null)
            {
                order.CustomerName = selectedCustomer.fullName;
            }

            order.Status = "Onay Bekliyor";
            order.RequestDate = order.RequestDate < new DateTime(1900, 1, 1) ? DateTime.Now : order.RequestDate;
            order.DeliveryDate = order.DeliveryDate < new DateTime(1900, 1, 1) ? DateTime.Now.AddDays(7) : order.DeliveryDate;

            c.CustomOrders.Add(order);
            c.SaveChanges();

            // ✅ Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Özel Sipariş Ekleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Yeni özel sipariş eklendi. Müşteri: {order.CustomerName}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ApproveOrder(int id)
        {
            var order = c.CustomOrders.Find(id);
            order.Status = "Üretimde";
            c.SaveChanges();

            // ✅ Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Sipariş Onaylama",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Sipariş onaylandı. Müşteri: {order.CustomerName}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult RejectOrder(int id)
        {
            var order = c.CustomOrders.Find(id);
            order.Status = "Reddedildi";
            c.SaveChanges();

            // ✅ Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Sipariş Reddetme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Sipariş reddedildi. Müşteri: {order.CustomerName}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult CompleteOrder(int id)
        {
            var order = c.CustomOrders.Find(id);
            if (order == null)
            {
                return HttpNotFound("Özel dikim talebi bulunamadı.");
            }

            order.Status = "Tamamlandı";
            c.SaveChanges();

            // ✅ Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Sipariş Tamamlama",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Sipariş tamamlandı. Müşteri: {order.CustomerName}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditOrder(int id)
        {
            var order = c.CustomOrders.Find(id);
            if (order == null)
                return HttpNotFound();

            ViewBag.Customers = new SelectList(c.Customers.ToList(), "id", "fullName", order.CustomerId);
            ViewBag.Categories = new SelectList(c.Categorys.ToList(), "categoryName", "categoryName", order.ProductType);
            return View(order);
        }

        [HttpPost]
        public ActionResult EditOrder(CustomOrder order)
        {
            var existingOrder = c.CustomOrders.Find(order.id);
            if (existingOrder == null)
                return HttpNotFound();

            if (!string.IsNullOrEmpty(order.Description))
                existingOrder.Description = order.Description;

            if (order.DeliveryDate != default(DateTime))
                existingOrder.DeliveryDate = order.DeliveryDate;

            if (order.Price > 0)
                existingOrder.Price = order.Price;

            if (order.Deposit > 0)
                existingOrder.Deposit = order.Deposit;

            if (!string.IsNullOrEmpty(order.ProductType))
                existingOrder.ProductType = order.ProductType;

            if (order.CustomerId != 0)
            {
                existingOrder.CustomerId = order.CustomerId;
                var selectedCustomer = c.Customers.FirstOrDefault(x => x.id == order.CustomerId);
                if (selectedCustomer != null)
                {
                    existingOrder.CustomerName = selectedCustomer.fullName;
                }
            }

            c.SaveChanges();

            // ✅ Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Sipariş Güncelleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Sipariş güncellendi. Müşteri: {existingOrder.CustomerName}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
