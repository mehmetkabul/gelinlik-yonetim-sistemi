using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gelinlik.Models.Class
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }
        public string Action { get; set; } // Yapılan işlem türü (Örneğin: "Kategori Ekleme", "Kategori Güncelleme", "Kategori Silme")
        public string PerformedBy { get; set; } // İşlemi yapan personelin adı
        public DateTime Date { get; set; } // İşlemin yapıldığı tarih ve saat
        public string Details { get; set; } // İşlemle ilgili açıklama (Örneğin: "Yeni kategori eklendi: Gelinlik")
    }
}