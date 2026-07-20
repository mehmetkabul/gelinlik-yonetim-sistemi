using Gelinlik.Models.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gelinlik.Controllers
{
    public class AppointmentController : Controller
    {
        Context c = new Context();

        public ActionResult Index()
        {
            // 1 gün geçmiş ve Beklemede olan randevuları sil
            var yesterday = DateTime.Today.AddDays(-1);
            var expiredAppointments = c.Appointments
                .Where(a => a.AppointmentDate <= yesterday && a.Status == "Beklemede")
                .ToList();

            if (expiredAppointments.Any())
            {
                c.Appointments.RemoveRange(expiredAppointments);
                c.SaveChanges();
            }

            // Geriye kalan randevular üzerinden takvim eventlerini oluştur
            var appointments = c.Appointments.Include("Customer").ToList();

            var events = appointments.Select(a => new
            {
                title = a.Customer.fullName + " | " + a.AppointmentTime + " | " + a.Notes + " | " + a.Status,
                start = a.AppointmentDate.ToString("yyyy-MM-dd"),
                color = a.Status == "Beklemede" ? "#ffc107" : a.Status == "Tamamlandı" ? "#28a745" : "#dc3545",
                url = a.Status == "Beklemede" ? Url.Action("EditAppointment", "Appointment", new { id = a.Id }) : null
            }).ToList();

            var filteredEvents = events.Select(e => new Dictionary<string, object>
    {
        { "title", e.title },
        { "start", e.start },
        { "color", e.color }
    }).ToList();

            foreach (var e in events.Where(x => x.url != null))
            {
                var dict = filteredEvents.FirstOrDefault(x => x["title"].ToString() == e.title && x["start"].ToString() == e.start);
                if (dict != null) dict.Add("url", e.url);
            }

            ViewBag.AppointmentEvents = Newtonsoft.Json.JsonConvert.SerializeObject(filteredEvents);
            return View();
        }


        [HttpGet]
        public ActionResult NewAppointment(string selectedDate, string selectedTime)
        {
            ViewBag.Customers = new SelectList(c.Customers, "Id", "FullName");

            var model = new Appointment();
            model.AppointmentDate = !string.IsNullOrEmpty(selectedDate) ? DateTime.Parse(selectedDate) : DateTime.Today;

            if (!string.IsNullOrEmpty(selectedTime))
            {
                if (TimeSpan.TryParse(selectedTime, out TimeSpan parsedTime))
                {
                    model.AppointmentTime = parsedTime;
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult NewAppointment(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = new SelectList(c.Customers, "Id", "FullName");
                return View(appointment);
            }

            // Sadece Beklemede olan randevuları kontrol et
            bool isConflict = c.Appointments.Any(a =>
                a.AppointmentDate == appointment.AppointmentDate &&
                a.AppointmentTime == appointment.AppointmentTime &&
                a.Status == "Beklemede"  // Sadece "Beklemede" durumundaki randevuları kontrol et
            );

            if (isConflict)
            {
                ViewBag.Message = "Bu tarih ve saatte başka bir randevu mevcut!";
                ViewBag.Customers = new SelectList(c.Customers, "Id", "FullName");
                return View(appointment);
            }

            c.Appointments.Add(appointment);
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var customer = c.Customers.Find(appointment.CustomerId);
            var customerName = customer != null ? customer.fullName : "Bilinmiyor";

            var activity = new Activity
            {
                Action = "Randevu Oluşturma",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Yeni randevu oluşturuldu. Müşteri: {customerName}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult EditAppointment(int id)
        {
            var appointment = c.Appointments.Include("Customer").FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return HttpNotFound();
            }

            ViewBag.Customers = new SelectList(c.Customers, "Id", "FullName", appointment.CustomerId);
            return View(appointment);
        }

        [HttpPost]
        public ActionResult EditAppointment(Appointment model)
        {
            var appointment = c.Appointments.Include("Customer").FirstOrDefault(a => a.Id == model.Id); // Müşteri bilgisiyle birlikte al
            if (appointment == null)
            {
                return HttpNotFound("Güncellenecek randevu bulunamadı.");
            }

            var userFullName = (string)Session["UserFullName"];
            var customerName = appointment.Customer != null ? appointment.Customer.fullName : "Bilinmiyor"; // Müşterinin adını kontrol et

            if (model.AppointmentDate == DateTime.MinValue || model.AppointmentDate == default(DateTime))
            {
                model.AppointmentDate = appointment.AppointmentDate;
            }

            if (model.AppointmentTime == null)
            {
                model.AppointmentTime = appointment.AppointmentTime;
            }

            if (string.IsNullOrEmpty(model.Status))
            {
                model.Status = appointment.Status;
            }

            try
            {
                appointment.CustomerId = model.CustomerId;
                appointment.AppointmentDate = model.AppointmentDate;
                appointment.AppointmentTime = model.AppointmentTime;
                appointment.Status = model.Status;

                c.SaveChanges();

                // Aktivite Kaydetme
                var activity = new Activity
                {
                    Action = "Randevu Güncelleme",
                    PerformedBy = userFullName,
                    Date = DateTime.Now,
                    Details = $"Randevu güncellendi. Müşteri: {customerName}, Yeni Durum: {model.Status}"
                };
                c.Activities.Add(activity);
                c.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Güncelleme sırasında bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Randevu başarıyla güncellendi!";
            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult DeleteAppointment(int id)
        {
            var appointment = c.Appointments.Include("Customer").FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return HttpNotFound();
            }

            return View(appointment);
        }

        [HttpPost]
        public ActionResult ConfirmDelete(int id)
        {
            var appointment = c.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }

            c.Appointments.Remove(appointment);
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Randevu Silme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Randevu ID: {id} silindi."
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
