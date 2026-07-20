using Gelinlik.Models.Class;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web;
using System.IO;
using Gelinlik.Attributes;

namespace Gelinlik.Controllers
{
    public class SettingsController : Controller
    {
        // GET: Settings
        Context c = new Context();
        [AuthorizeRole("Admin")]
        [HttpGet]
        public ActionResult Edit()
        {
            var setting = c.SiteSettings.FirstOrDefault();
            if (setting == null)
            {
                setting = new Setting
                {
                    CompanyName = "Rental Manager",
                    LogoPath = null
                };
                c.SiteSettings.Add(setting);
                c.SaveChanges();
            }
            return View(setting);
        }

        [HttpPost]
        public ActionResult Edit(Setting model,HttpPostedFileBase logoFile)
        {
            var setting = c.SiteSettings.FirstOrDefault();
            if (setting == null) return HttpNotFound();

            setting.CompanyName = model.CompanyName;

            if (logoFile != null && logoFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(logoFile.FileName);
                string path = Path.Combine(Server.MapPath("~/web-images/"), fileName);
                logoFile.SaveAs(path);
                setting.LogoPath = "/web-images/" + fileName;
            }

            c.SaveChanges();
            TempData["Success"] = "Site ayarları başarıyla güncellendi.";
            return RedirectToAction("Edit");
        }

        public ActionResult DeleteLogo()
        {
            var setting = c.SiteSettings.FirstOrDefault();
            if (setting == null || string.IsNullOrEmpty(setting.LogoPath)) return HttpNotFound();

            string fullPath = Server.MapPath(setting.LogoPath);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            setting.LogoPath = null;
            c.SaveChanges();

            return RedirectToAction("Edit");
        }
    }
}