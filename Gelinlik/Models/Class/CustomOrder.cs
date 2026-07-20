using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gelinlik.Models.Class
{
    public class CustomOrder
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Müşteri Adı")]
        public string CustomerName { get; set; }

        [Display(Name = "Kategori")]
        public string ProductType { get; set; } // Kategori adı (string olarak tutulacak)

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Talep Tarihi")]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Termin Tarihi")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        [Display(Name = "Kapora")]
        public decimal Deposit { get; set; }

        [Display(Name = "Durum")]
        public string Status { get; set; } // Onay Bekliyor, Üretimde, Reddedildi, Tamamlandı

        [Display(Name = "Müşteri")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }

}