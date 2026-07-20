using Gelinlik.Models.Class;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;

namespace Gelinlik.Controllers
{
    public class AdminController : Controller
    {
        Context c = new Context();

        public ActionResult Index(string filter = "Today", int page = 1)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var now = DateTime.Now;

            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var yearStart = new DateTime(today.Year, 1, 1);

            IQueryable<Appointment> appointments = c.Appointments.Include(x => x.Customer);
            IQueryable<Rental> rentals = c.Rentals.Where(x => x.IsReturned == false);

            DateTime filterStart = today;
            DateTime filterEnd = today.AddDays(1);

            switch (filter)
            {
                case "Today":
                    filterStart = today;
                    filterEnd = today.AddDays(1);
                    ViewBag.AppointmentTitle = "Bugünün Randevuları";
                    break;
                case "ThisWeek":
                    filterStart = weekStart;
                    filterEnd = weekStart.AddDays(7);
                    ViewBag.AppointmentTitle = "Bu Haftaki Randevular";
                    break;
                case "ThisMonth":
                    filterStart = monthStart;
                    filterEnd = monthStart.AddMonths(1);
                    ViewBag.AppointmentTitle = "Bu Ayki Randevular";
                    break;
                case "ThisYear":
                    filterStart = yearStart;
                    filterEnd = yearStart.AddYears(1);
                    ViewBag.AppointmentTitle = "Bu Yılki Randevular";
                    break;
            }

            // ➤ 1 gün geçmiş ve hala beklemede olan randevuları sil
            var outdatedAppointments = c.Appointments
                .Where(x => x.AppointmentDate < today && x.Status == "Beklemede")
                .ToList();

            if (outdatedAppointments.Any())
            {
                c.Appointments.RemoveRange(outdatedAppointments);
                c.SaveChanges();
            }

            appointments = appointments.Where(x => x.AppointmentDate >= filterStart && x.AppointmentDate < filterEnd);
            rentals = rentals.Where(x => x.StartDate >= filterStart && x.StartDate < filterEnd && x.IsReturned == false);

            ViewBag.TotalDeposits = c.Rentals
                .Where(x => x.StartDate >= filterStart && x.StartDate < filterEnd && x.IsReturned == false)
                .Sum(x => (decimal?)x.Deposit) ?? 0;

            // Sayfalama
            int pageSize = 5;
            var allActivities = c.Activities.OrderByDescending(a => a.Date).ToList();
            var paginatedActivities = allActivities.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling((double)allActivities.Count / pageSize);

            // Bildirimler
            var overdueRentals = c.Rentals
                .Include(r => r.Customer)
                .Where(r => !r.IsReturned && r.EndDate < today)
                .ToList();

            var dueTomorrowRentals = c.Rentals
                .Include(r => r.Customer)
                .Where(r => !r.IsReturned && DbFunctions.TruncateTime(r.EndDate) == tomorrow)
                .ToList();

            var dueTodayRentals = c.Rentals
                .Include(r => r.Customer)
                .Where(r => !r.IsReturned && DbFunctions.TruncateTime(r.EndDate) == today)
                .ToList();

            var todaysAppointments = c.Appointments
                .Include(a => a.Customer)
                .Where(a => DbFunctions.TruncateTime(a.AppointmentDate) == today)
                .ToList();

            var lowStockProducts = c.Products
                .Where(p => p.stock > 0 && p.stock <= 3)
                .ToList();

            var rentalsWithoutBond = c.Rentals
                .Include(r => r.Customer)
                .Where(r => string.IsNullOrEmpty(r.BondPath))
                .ToList();
            var pendingCustomOrders = c.CustomOrders
                .Where(x => x.Status == "Onay Bekliyor")
                .OrderBy(x => x.DeliveryDate)
                .ToList();


            var model = new HomeViewModel
            {
                Appointments = c.Appointments.Include(x => x.Customer)
         .OrderBy(x => x.AppointmentDate)
         .ThenBy(x => x.AppointmentTime)
         .ToList(),
                Activities = paginatedActivities,
                OverdueRentals = overdueRentals,
                DueTomorrowRentals = dueTomorrowRentals,
                DueTodayRentals = dueTodayRentals,
                TodaysAppointments = todaysAppointments,
                LowStockProducts = lowStockProducts,
                RentalsWithoutBond = rentalsWithoutBond,
                PendingCustomOrders = pendingCustomOrders // <-- bu satırı ekle
            };

            ViewBag.TotalCustomers = c.Customers.Count();
            ViewBag.AvailableProducts = c.Products.Count(x => x.stock > 0);
            ViewBag.ActiveRentals = c.Rentals.Count(x => x.IsReturned == false);
            ViewBag.TodaysAppointments = appointments.Count();
            ViewBag.PendingPayments = c.Rentals.Where(x => x.IsReturned == false).Sum(x => (decimal?)x.Deposit) ?? 0;
            ViewBag.Filter = filter;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(model);
        }




        [HttpGet]
        [ActionName("GetChartData")]
        public JsonResult GetChartData(string[] types, string filter)
        {
            var today = DateTime.Today;
            DateTime start = today;
            DateTime end = today.AddDays(1);

            switch (filter)
            {
                case "ThisWeek":
                    int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    start = today.AddDays(-1 * diff).Date;
                    end = start.AddDays(7);
                    break;

                case "ThisMonth":
                    start = new DateTime(today.Year, today.Month, 1);
                    end = start.AddMonths(1);
                    break;

                case "ThisYear":
                    start = new DateTime(today.Year, 1, 1);
                    end = start.AddYears(1);
                    break;
            }

            List<string> labels = new List<string>();
            var chartData = new Dictionary<string, Dictionary<string, decimal>>();


            if (filter == "Today" || filter == "ThisWeek" || filter == "ThisMonth")
            {
                for (DateTime date = start; date < end; date = date.AddDays(1))
                    labels.Add(date.ToString("dd MMM"));
            }
            else if (filter == "ThisYear")
            {
                for (int i = 1; i <= 12; i++)
                    labels.Add(new DateTime(today.Year, i, 1).ToString("MMM"));
            }

            // SATIŞ
            if (types.Contains("sales"))
            {
                var sales = c.Sales.Where(x => x.SaleDate >= start && x.SaleDate < end).ToList();
                chartData["Satış"] = labels.ToDictionary(label => label, label => 0m);

                foreach (var sale in sales)
                {
                    string key = filter == "ThisYear" ? sale.SaleDate.ToString("MMM") : sale.SaleDate.ToString("dd MMM");
                    if (chartData["Satış"].ContainsKey(key))
                        chartData["Satış"][key] += sale.TotalPrice;
                }
            }

            // KİRALAMA
            if (types.Contains("rentals"))
            {
                var rentals = c.Rentals.Where(x => x.StartDate >= start && x.StartDate < end).ToList();
                chartData["Kiralama"] = labels.ToDictionary(label => label, label => 0m);

                foreach (var rental in rentals)
                {
                    string key = filter == "ThisYear" ? rental.StartDate.ToString("MMM") : rental.StartDate.ToString("dd MMM");
                    if (chartData["Kiralama"].ContainsKey(key))
                        chartData["Kiralama"][key] += rental.Deposit;
                }
            }

            // ÖDEME
            if (types.Contains("payments"))
            {
                var payments = c.Rentals.Where(x => x.Deposit > 0 && x.StartDate >= start && x.StartDate < end).ToList();
                chartData["Ödeme"] = labels.ToDictionary(label => label, label => 0m);

                foreach (var payment in payments)
                {
                    string key = filter == "ThisYear" ? payment.StartDate.ToString("MMM") : payment.StartDate.ToString("dd MMM");
                    if (chartData["Ödeme"].ContainsKey(key))
                        chartData["Ödeme"][key] += payment.Deposit;
                }
            }

            var datasets = chartData.Select(kvp => new
            {
                label = kvp.Key,
                data = labels.Select(l => kvp.Value.ContainsKey(l) ? kvp.Value[l] : 0m).ToArray()
            }).ToList();

            return Json(new { labels, datasets }, JsonRequestBehavior.AllowGet);
        }







    }
}
