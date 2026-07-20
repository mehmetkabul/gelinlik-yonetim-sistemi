using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gelinlik.Models.Class
{
    public class Rental
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int CustomerId { get; set; } // Müşteri ID
        
        public int? ProductId { get; set; } // Ürün ID - artık nullable ve required değil
        [Required]
        public DateTime StartDate { get; set; } // Kiralama Başlangıç Tarihi

        [Required]
        public DateTime EndDate { get; set; } // Kiralama Bitiş Tarihi

        [Required]
        public decimal Deposit { get; set; } // Kapora

        public bool IsReturned { get; set; } = false; // Ürün iade edildi mi?

        // İLAVE EDİLEN KISIMLAR (Yeni Property'ler)
        public string DamageDetails { get; set; } // Hasar açıklaması
        public string DamageImagePath { get; set; } // Yüklenen hasar fotoğrafının yolu
        public string DamageReporter { get; set; }

        // Navigation Properties Ekleyelim
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products Product { get; set; }

        public string BondPath { get; set; } // Senet dosyasının yolu

    }
}