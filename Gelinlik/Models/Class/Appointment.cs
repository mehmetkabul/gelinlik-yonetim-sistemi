using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gelinlik.Models.Class
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }  // Saat bilgisi
        [Required]
        public string Status { get; set; } = "Beklemede";

        public string Notes { get; set; }  // Ek notlar

        public virtual Customer Customer { get; set; }  // Müşteri ilişkisi
    }
}