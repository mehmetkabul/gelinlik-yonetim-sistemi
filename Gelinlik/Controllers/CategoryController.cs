using Gelinlik.Attributes;
using Gelinlik.Models.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gelinlik.Controllers
{
    public class CategoryController : Controller
    {
        Context c = new Context();

        [AuthorizeRole("Admin")]
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
        public ActionResult NewCategory()
        {
            var model = new HomeViewModel
            {
                Category = new Category(),
                Categories = c.Categorys.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult NewCategory(HomeViewModel model)
        {
            if (model == null || model.Category == null)
            {
                model = new HomeViewModel { Categories = c.Categorys.ToList() };
                return View(model);
            }

            c.Categorys.Add(model.Category);
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Kategori Ekleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Yeni kategori eklendi: {model.Category.categoryName}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Category");
        }

        public ActionResult CategoryFind(int id)
        {
            var category = c.Categorys.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            var model = new HomeViewModel
            {
                Category = category
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateCategory(HomeViewModel model)
        {
            var uc = c.Categorys.Find(model.Category.id);
            if (uc == null)
            {
                return HttpNotFound();
            }

            string oldName = uc.categoryName;
            uc.categoryName = model.Category.categoryName;
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Kategori Güncelleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Kategori güncellendi: {oldName} => {model.Category.categoryName}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Category");
        }

        public ActionResult RemoveCategory(int id)
        {
            var rc = c.Categorys.Find(id);
            if (rc == null)
            {
                return HttpNotFound();
            }

            c.Categorys.Remove(rc);
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"];
            var activity = new Activity
            {
                Action = "Kategori Silme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Kategori silindi: {rc.categoryName}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Category");
        }
    }
}
