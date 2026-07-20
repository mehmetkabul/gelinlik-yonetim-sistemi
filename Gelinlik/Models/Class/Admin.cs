using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gelinlik.Models.Class
{
    public class Admin
    {
        [Key]
        public int id { get; set; }
        public string adminUser { get; set; }
        public string adminPassword { get; set; }
    }
}