using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gelinlik.Models.Class
{
    public class Products
    {
        [Key]
        public int id { get; set; }  // nullable kaldırıldı
        public string name { get; set; }
        public int CategoryId { get; set; } // ✅ Kategori ID olarak eklendi
        public virtual Category Category { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public int rentalPrice { get; set; }
        public int stock { get; set; }
        public bool isAvailable { get; set; } = true; // Varsayılan olarak true
        public DateTime? createdAt { get; set; }
        public DateTime? updateAt { get; set; }

        

    }
}