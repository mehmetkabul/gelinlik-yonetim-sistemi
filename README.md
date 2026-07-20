# Gelinlik Yönetim Sistemi

Gelinlik/abiye mağazaları için geliştirilmiş, web tabanlı bir işletme
yönetim sistemi (ERP). Ürün stok takibi, müşteri yönetimi, kiralama ve
satış işlemleri, randevu planlama, özel dikim siparişleri ve personel
yönetimini tek panelde birleştirir.

## Özellikler

- 📊 **Dashboard** — Dönemsel filtreleme (Bugün/Hafta/Ay/Yıl), KPI özetleri
  (müşteri sayısı, aktif kiralama, toplam kapora, uygun ürün), Chart.js ile
  satış/kiralama/ödeme grafikleri
- 🔔 **Akıllı Bildirimler** — Geciken kiralamalar, yarın/bugün dönecek
  teslimler, düşük stok uyarıları, senetsiz kiralamalar, bekleyen özel
  siparişler otomatik tespit edilir
- 📦 **Ürün & Stok Yönetimi** — Kategori bazlı ürün CRUD işlemleri
- 🤵 **Kiralama Yönetimi** — Kiralama oluşturma, iade, süre uzatma; senet
  ve hasar raporu (fotoğraf + açıklama) yükleme/arşivleme
- 🧾 **Satış Yönetimi** — Satış kaydı ve otomatik stok düşümü
- 📅 **Randevu Sistemi** — Randevu CRUD, süresi geçen "Beklemede"
  randevuların otomatik temizlenmesi
- ✂️ **Özel Dikim Siparişleri** — Durum akışı: Onay Bekliyor → Üretimde →
  Tamamlandı / Reddedildi
- 👥 **Personel Yönetimi** — Rol bazlı yetkilendirme (Admin / Personel)
- 📝 **Denetim Kaydı (Activity Log)** — Tüm kritik işlemler loglanır

## Kullanılan Teknolojiler

| Katman | Teknoloji |
|---|---|
| Backend | ASP.NET MVC 5, C#, .NET Framework 4.7.2 |
| ORM | Entity Framework 6.5 (Code First + Migrations) |
| Veritabanı | SQL Server LocalDB |
| Frontend | Razor Views (.cshtml), Bootstrap 5.3, jQuery 3.7, Chart.js |
| Raporlama | Rotativa (wkhtmltopdf) — senet/rapor PDF çıktıları |
| Kimlik Doğrulama | Forms Authentication + Session tabanlı rol kontrolü |

## Mimari

Proje katmanlı MVC mimarisiyle geliştirildi:
