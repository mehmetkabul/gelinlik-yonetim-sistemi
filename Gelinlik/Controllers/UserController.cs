using Gelinlik.Attributes;
using Gelinlik.Models.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Gelinlik.Controllers
{
    [AuthorizeRole("Admin")] // Sadece Admin kullanıcıları bu controllera erişebilir.
    public class UserController : Controller
    {
        Context c = new Context();
        [AuthorizeRole("Admin")]
        // Kullanıcı Listesi
        public ActionResult Index()
        {
            var users = c.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public ActionResult NewUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewUser(User user)
        {
            if (string.IsNullOrEmpty(user.FullName) || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Role))
            {
                ViewBag.Error = "Lütfen tüm alanları doldurun.";
                return View();
            }

            c.Users.Add(user);
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Kullanıcı Ekleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Yeni Kullanıcı Eklendi: Ad: {user.FullName}, Kullanıcı Adı: {user.Username}, Rol: {user.Role}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ConfirmRemoveUser(int id)
        {
            var user = c.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserId = id;
            return View();
        }

        public ActionResult RemoveUser(int id)
        {
            var user = c.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            string removedUserName = user.FullName; // Kullanıcı bilgilerini sakla
            string removedUsername = user.Username;
            string removedRole = user.Role;

            c.Users.Remove(user);
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Kullanıcı Silme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Kullanıcı Silindi: Ad: {removedUserName}, Kullanıcı Adı: {removedUsername}, Rol: {removedRole}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult EditUser(int id)
        {
            var user = c.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            ViewBag.Roles = new SelectList(new List<string> { "Admin", "Personel" }, user.Role);
            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(User user)
        {
            var existingUser = c.Users.Find(user.Id);
            if (existingUser == null)
                return HttpNotFound();

            // Eski verileri sakla
            string oldFullName = existingUser.FullName;
            string oldUsername = existingUser.Username;
            string oldRole = existingUser.Role;

            // Güncelleme işlemi - Eğer boş bırakılırsa eski değer korunur
            existingUser.FullName = !string.IsNullOrEmpty(user.FullName) ? user.FullName : existingUser.FullName;
            existingUser.Username = !string.IsNullOrEmpty(user.Username) ? user.Username : existingUser.Username;
            existingUser.Password = !string.IsNullOrEmpty(user.Password) ? user.Password : existingUser.Password;
            existingUser.Role = !string.IsNullOrEmpty(user.Role) ? user.Role : existingUser.Role;

            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Kullanıcı Güncelleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Kullanıcı Güncellendi: Eski Ad: {oldFullName}, Yeni Ad: {existingUser.FullName}, " +
                          $"Eski Kullanıcı Adı: {oldUsername}, Yeni Kullanıcı Adı: {existingUser.Username}, " +
                          $"Eski Rol: {oldRole}, Yeni Rol: {existingUser.Role}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
