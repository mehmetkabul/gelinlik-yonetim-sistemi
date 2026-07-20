using Gelinlik.Models.Class;
using System.Web.Mvc;
using System.Linq;
using Antlr.Runtime.Misc;
using System;
using System.Web;
using System.IO;
using System.Data.Entity;

namespace Gelinlik.Controllers
{
    public class RentalController : Controller
    {
        // GET: Rental
        Context c = new Context();
        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Customers = c.Customers.ToList(),
                Products = c.Products.ToList(),
                Categories = c.Categorys.ToList(),
                Rentals = c.Rentals.Include("Customer").Include("Product").ToList(),
            };
            

            return View(model);
        }

        [HttpGet]
        public ActionResult NewRental()
        {
            // Debug için tüm ürünleri önce listeye alalım
            var allProducts = c.Products.ToList();
            
            var model = new HomeViewModel
            {
                Customers = c.Customers.ToList(),
                Categories = c.Categorys.ToList(),
                Products = allProducts, // Şimdilik filtrelemeyi kaldıralım
                Rental = new Rental()
            };

            // Debug bilgisi ekleyelim
            if (allProducts == null || !allProducts.Any())
            {
                ViewBag.Message = "Veritabanında hiç ürün bulunmamaktadır.";
            }
            else
            {
                ViewBag.Message = $"Toplam {allProducts.Count} ürün bulundu.";
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult NewRental(HomeViewModel model)
        {
            if (model == null || model.Rental == null)
            {
                return HttpNotFound();
            }

            var product = c.Products.Find(model.Rental.ProductId);
            var customer = c.Customers.Find(model.Rental.CustomerId); // Müşteri bilgilerini de alalım

            // Eğer ürün yoksa veya stoğu sıfırsa hata mesajı göster
            if (product == null || product.stock <= 0)
            {
                ViewBag.StockError = "Seçilen ürünün stoğu bulunmamaktadır. Lütfen başka bir ürün seçin.";
                model.Customers = c.Customers.ToList();
                model.Products = c.Products.Where(x => x.isAvailable).ToList();
                return View(model);
            }

            // Tarih kontrolü
            if (model.Rental.EndDate < model.Rental.StartDate)
            {
                ViewBag.DateError = "Kiralama bitiş tarihi, başlangıç tarihinden önce olamaz!";
                model.Customers = c.Customers.ToList();
                model.Products = c.Products.Where(x => x.isAvailable).ToList();
                return View(model);
            }

            model.Rental.IsReturned = false;

            // Kiralama işlemi ekleniyor
            c.Rentals.Add(model.Rental);

            // Ürünün stok adedini düşür
            product.stock -= 1;

            // Eğer stok tamamen bittiyse, ürünü kiralamaya kapat
            if (product.stock == 0)
            {
                product.isAvailable = false;
            }

            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Kiralama",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Yeni Kiralama Oluşturuldu: Müşteri: {customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {product?.name ?? "Ürün Bulunamadı"}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Rental");
        }




        // 4. Ürünü İade Etme
        public ActionResult ReturnRental(int id)
        {
            var rental = c.Rentals.Include(r => r.Customer).Include(r => r.Product).FirstOrDefault(r => r.id == id);
            if (rental == null) return HttpNotFound();

            rental.IsReturned = true;

            // Ürün tekrar uygun hale getirildi
            var product = rental.Product; // Ürünü ilişki üzerinden alıyoruz
            if (product != null)
            {
                product.stock = product.stock + 1;
                product.isAvailable = true;
            }

            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Ürün İade",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Ürün İade Edildi: Müşteri: {rental.Customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {product?.name ?? "Ürün Bulunamadı"}"
            };
            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Rental");
        }



        // Kira Kaydı Silme
        public ActionResult RemoveRental(int id)
        {
            // İlişkili verileri tam olarak çekiyoruz
            var cr = c.Rentals
                      .Include(r => r.Customer)  // Müşteri ilişkisini dahil ediyoruz
                      .Include(r => r.Product)   // Ürün ilişkisini dahil ediyoruz
                      .FirstOrDefault(r => r.id == id);

            if (cr == null)
            {
                return HttpNotFound();
            }

            string customerName = cr.Customer != null ? cr.Customer.fullName : "Müşteri Bulunamadı";
            string productName = cr.Product != null ? cr.Product.name : "Ürün Bulunamadı";

            c.Rentals.Remove(cr);

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Kiralama",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Kiralama Kaydı Silindi: Müşteri: {customerName}, Ürün: {productName}"
            };

            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Rental");
        }

        public ActionResult ConfirmRemoveRental(int id)
        {
            var rental = c.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }

            ViewBag.RentalId = id;
            return View();
        }

        public ActionResult RentalFind(int id)
        {
            var ur = c.Rentals.Find(id);
            if (ur == null) return HttpNotFound();

            var model = new HomeViewModel
            {
                Rental = ur,
                Customers = c.Customers.ToList(),
                Products = c.Products.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateRental(HomeViewModel model)
        {
            var ur = c.Rentals.Include(r => r.Customer).Include(r => r.Product).FirstOrDefault(r => r.id == model.Rental.id);
            if (ur == null) return HttpNotFound();

            // Güncelleme öncesi eski müşteri ve ürün bilgilerini alalım
            var previousCustomerName = ur.Customer?.fullName ?? "Müşteri Bulunamadı";
            var previousProductName = ur.Product?.name ?? "Ürün Bulunamadı";

            // Eğer yeni değer boş değilse, eski değeri koru
            ur.CustomerId = model.Rental.CustomerId != 0 ? model.Rental.CustomerId : ur.CustomerId;
            ur.ProductId = model.Rental.ProductId != 0 ? model.Rental.ProductId : ur.ProductId;
            ur.StartDate = model.Rental.StartDate != default(DateTime) ? model.Rental.StartDate : ur.StartDate;
            ur.EndDate = model.Rental.EndDate != default(DateTime) ? model.Rental.EndDate : ur.EndDate;
            ur.Deposit = model.Rental.Deposit > 0 ? model.Rental.Deposit : ur.Deposit;
            ur.IsReturned = model.Rental.IsReturned; // Boolean için ekstra kontrol gerekmiyor

            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Kiralama Güncelleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Kiralama Güncellendi: Müşteri: {previousCustomerName} -> Yeni Müşteri: {ur.Customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {previousProductName} -> Yeni Ürün: {ur.Product?.name ?? "Ürün Bulunamadı"}"
            };

            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index", "Rental");
        }


        public JsonResult GetProductsByCategory(int categoryId)
        {
            var products = c.Products
                            .Where(p => p.CategoryId == categoryId && p.isAvailable)
                            .Select(p => new { p.id, p.name })
                            .ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExtendRental(int id)
        {
            var rental = c.Rentals.Find(id);
            if (rental == null || rental.IsReturned)
                return HttpNotFound();

            return View(rental);
        }

        [HttpPost]
        public ActionResult ExtendRental(Rental updatedRental)
        {
            var rental = c.Rentals.Include(r => r.Customer).Include(r => r.Product).FirstOrDefault(r => r.id == updatedRental.id);
            if (rental == null || rental.IsReturned)
                return HttpNotFound();

            // Eski bitiş tarihini saklayalım
            var previousEndDate = rental.EndDate;

            // Bitiş tarihini güncelle
            if (updatedRental.EndDate > rental.EndDate)
            {
                rental.EndDate = updatedRental.EndDate;
                c.SaveChanges();

                // Aktivite Kaydetme
                var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
                var activity = new Activity
                {
                    Action = "Kiralama Süresi Uzatma",
                    PerformedBy = userFullName,
                    Date = DateTime.Now,
                    Details = $"Kiralama Süresi Uzatıldı: Müşteri: {rental.Customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {rental.Product?.name ?? "Ürün Bulunamadı"}, Eski Bitiş Tarihi: {previousEndDate.ToShortDateString()}, Yeni Bitiş Tarihi: {rental.EndDate.ToShortDateString()}"
                };

                c.Activities.Add(activity);
                c.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult UploadNote(int id)
        {
            var rental = c.Rentals.Find(id);
            if (rental == null) return HttpNotFound();
            return View(rental);
        }

        [HttpPost]
        public ActionResult UploadNote(int id, HttpPostedFileBase noteFile)
        {
            var rental = c.Rentals.Include(r => r.Customer).Include(r => r.Product).FirstOrDefault(r => r.id == id);
            if (rental == null || noteFile == null) return HttpNotFound();

            string fileName = System.IO.Path.GetFileName(noteFile.FileName);
            string path = Server.MapPath("~/web-images/" + fileName);
            noteFile.SaveAs(path);

            rental.BondPath = "/web-images/" + fileName;
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Senet Yükleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Senet Yüklendi: Müşteri: {rental.Customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {rental.Product?.name ?? "Ürün Bulunamadı"}, Dosya Adı: {fileName}"
            };

            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ViewNote(int id)
        {
            var rental = c.Rentals.Include(r => r.Customer).Include(r => r.Product).FirstOrDefault(r => r.id == id);
            if (rental == null || string.IsNullOrEmpty(rental.BondPath))
                return HttpNotFound();

            string fullPath = Server.MapPath(rental.BondPath);
            string fileExtension = System.IO.Path.GetExtension(fullPath).ToLower();

            string contentType = "application/octet-stream";
            if (fileExtension == ".jpg" || fileExtension == ".jpeg") contentType = "image/jpeg";
            else if (fileExtension == ".png") contentType = "image/png";
            else if (fileExtension == ".pdf") contentType = "application/pdf";

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Senet Görüntüleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Senet Görüntülendi: Müşteri: {rental.Customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {rental.Product?.name ?? "Ürün Bulunamadı"}, Dosya Yolu: {rental.BondPath}"
            };

            c.Activities.Add(activity);
            c.SaveChanges();

            return File(fullPath, contentType);
        }


        public ActionResult DeleteNote(int id)
        {
            var rental = c.Rentals.Include(r => r.Customer).Include(r => r.Product).FirstOrDefault(r => r.id == id);
            if (rental == null || string.IsNullOrEmpty(rental.BondPath))
                return HttpNotFound();

            string fullPath = Server.MapPath(rental.BondPath);
            bool fileDeleted = false;

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                fileDeleted = true;
            }

            rental.BondPath = null;
            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Senet Silme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Senet Silindi: Müşteri: {rental.Customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {rental.Product?.name ?? "Ürün Bulunamadı"}, Dosya Silindi: {(fileDeleted ? "Başarılı" : "Başarısız")}"
            };

            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult DamageReport(int id)
        {
            var rental = c.Rentals.Find(id);
            if (rental == null) return HttpNotFound();

            var model = new Rental
            {
                id = rental.id
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveDamageReport(int id, string damageDetails, HttpPostedFileBase damageImage)
        {
            var rental = c.Rentals.Include(r => r.Customer).Include(r => r.Product)
                                  .FirstOrDefault(r => r.id == id);
            if (rental == null) return HttpNotFound();

            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";  // Kullanıcı adını alıyoruz
            string previousImagePath = rental.DamageImagePath;  // Önceki hasar fotoğraf yolunu sakla
            bool isImageUpdated = false;  // Görselin değiştirilip değiştirilmediğini takip etmek için
            string uploadedFileName = "Yüklenmedi";  // Yüklenen dosya adını saklamak için

            rental.DamageDetails = damageDetails;
            rental.DamageReporter = userFullName;  // Bildiren personelin adını kaydet

            if (damageImage != null && damageImage.ContentLength > 0)
            {
                string fileName = System.IO.Path.GetFileName(damageImage.FileName);
                uploadedFileName = fileName;  // Aktivite kaydı için dosya adını sakla
                string path = System.IO.Path.Combine(Server.MapPath("~/web-images/damagereports"), fileName);
                damageImage.SaveAs(path);

                rental.DamageImagePath = "/web-images/damagereports/" + fileName;
                isImageUpdated = true;  // Görsel güncellendi
            }

            c.SaveChanges();

            // Aktivite Kaydetme
            var activity = new Activity
            {
                Action = "Hasar Bildirimi",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Hasar Bildirimi Yapıldı: Müşteri: {rental.Customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {rental.Product?.name ?? "Ürün Bulunamadı"}, Hasar Detayı: {damageDetails}, Fotoğraf Güncellendi: {(isImageUpdated ? "Evet" : "Hayır")}, Dosya Adı: {uploadedFileName}"
            };

            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("DamageReports", "Rental");
        }



        public ActionResult DamageReports()
        {
            var damageReports = c.Rentals
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .Where(r => !string.IsNullOrEmpty(r.DamageDetails))
                .ToList();

            return View(damageReports);
        }

        public ActionResult DamageReportDetails(int id)
        {
            var rental = c.Rentals
                          .Include(r => r.Customer)
                          .Include(r => r.Product)
                          .FirstOrDefault(r => r.id == id);

            if (rental == null)
                return HttpNotFound();

            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";

            // Aktivite Kaydetme
            var activity = new Activity
            {
                Action = "Hasar Raporu Görüntüleme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Hasar Raporu Görüntülendi: Müşteri: {rental.Customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {rental.Product?.name ?? "Ürün Bulunamadı"}, Yüklenen Görsel: {(string.IsNullOrEmpty(rental.DamageImagePath) ? "Yüklenmemiş" : rental.DamageImagePath)}"
            };

            c.Activities.Add(activity);
            c.SaveChanges();

            return View(rental);
        }


        // Hasar Kaydını Silme Onayı Sayfası (GET)
        [HttpGet]
        public ActionResult ConfirmRemoveDamageReport(int id)  // İsim burada düzeltilmeli
        {
            var rental = c.Rentals.Find(id);
            if (rental == null || string.IsNullOrEmpty(rental.DamageDetails))
                return HttpNotFound();

            ViewBag.RentalId = id;
            return View("ConfirmRemoveDamageReport");  // Görüntülenecek View adı doğru belirtilmeli
        }

        // Hasar Kaydını Silme İşlemi (POST)
        [HttpPost]
        public ActionResult RemoveDamage(int id)
        {
            var rental = c.Rentals.Include(r => r.Customer).Include(r => r.Product).FirstOrDefault(r => r.id == id);
            if (rental == null)
                return HttpNotFound();

            // Hasar bilgilerinin silinmeden önceki durumu kaydetmek için
            var oldDamageDetails = rental.DamageDetails;
            var oldDamageImagePath = rental.DamageImagePath;
            var oldDamageReporter = rental.DamageReporter;

            // Hasar bilgilerini temizle
            rental.DamageDetails = null;
            rental.DamageImagePath = null;
            rental.DamageReporter = null;

            c.SaveChanges();

            // Aktivite Kaydetme
            var userFullName = (string)Session["UserFullName"] ?? "Bilinmeyen Kullanıcı";
            var activity = new Activity
            {
                Action = "Hasar Kaydı Silme",
                PerformedBy = userFullName,
                Date = DateTime.Now,
                Details = $"Hasar Kaydı Silindi: Müşteri: {rental.Customer?.fullName ?? "Müşteri Bulunamadı"}, Ürün: {rental.Product?.name ?? "Ürün Bulunamadı"}, Silinen Fotoğraf: {oldDamageImagePath ?? "Fotoğraf Bulunamadı"}, Bildiren: {oldDamageReporter ?? "Bildiren Bulunamadı"}"
            };

            c.Activities.Add(activity);
            c.SaveChanges();

            return RedirectToAction("DamageReports", "Rental");
        }




    }
}