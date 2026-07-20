using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gelinlik.Models.Class
{
    public class Sales
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int CustomerId { get; set; }  // Müşteri ID

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } // Müşteri Nesnesi

        [Required]
        public int ProductId { get; set; }  // Ürün ID

        [ForeignKey("ProductId")]
        public virtual Products Product { get; set; } // Ürün Nesnesi

        [Required]
        public DateTime SaleDate { get; set; }  // Satış Tarihi

        [Required]
        public int Quantity { get; set; }  // Satılan Adet

        [Required]
        public decimal TotalPrice { get; set; }  // Toplam Fiyat
    }
}
