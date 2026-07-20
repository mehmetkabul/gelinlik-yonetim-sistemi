using Gelinlik.Models.Class;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Gelinlik.Controllers
{
    public class LoginController : Controller
    {
        Context c = new Context();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string adminUser, string adminPassword)
        {
            var bilgiler = c.Admins.FirstOrDefault(x => x.adminUser == adminUser && x.adminPassword == adminPassword);

            if (bilgiler != null)
            {
                FormsAuthentication.SetAuthCookie(bilgiler.adminUser, false);
                Session["adminUser"] = bilgiler.adminUser.ToString();
                Session["UserRole"] = "Admin";
                Session["UserFullName"] = bilgiler.adminUser;

                // ✅ Aktivite Kaydetme (Admin Girişi)
                var activity = new Activity
                {
                    Action = "Giriş Yapma",
                    PerformedBy = bilgiler.adminUser,
                    Date = DateTime.Now,
                    Details = $"Admin kullanıcı {bilgiler.adminUser} sisteme giriş yaptı."
                };
                c.Activities.Add(activity);
                c.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.ErrorMessage = "Hatalı kullanıcı adı veya şifre!";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = c.Users.FirstOrDefault(x => x.Username == username && x.Password == password);

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Username, false);
                Session["UserRole"] = user.Role;
                Session["UserId"] = user.Id;
                Session["UserFullName"] = user.FullName;

                // ✅ Aktivite Kaydetme (Personel Girişi)
                var activity = new Activity
                {
                    Action = "Giriş Yapma",
                    PerformedBy = user.FullName,
                    Date = DateTime.Now,
                    Details = $"{user.FullName} ({user.Role}) sisteme giriş yaptı."
                };
                c.Activities.Add(activity);
                c.SaveChanges();

                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Admin");
                else if (user.Role == "Personel")
                    return RedirectToAction("Index", "Admin");
            }

            ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı!";
            return View("Index");
        }

        public ActionResult LogOut()
        {
            var userFullName = (string)Session["UserFullName"];

            // ✅ Aktivite Kaydetme (Çıkış Yapma)
            var activity = new Activity
            {
                Action = "Çıkış Yapma",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"{userFullName} sistemden çıkış yaptı."
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
