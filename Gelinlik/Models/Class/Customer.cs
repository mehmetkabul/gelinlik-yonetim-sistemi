using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gelinlik.Models.Class
{
    public class Customer
    {
        [Key]
        public int id { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updateAt { get; set; }
    }
}