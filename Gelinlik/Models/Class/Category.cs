using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gelinlik.Models.Class
{
    public class Category
    {
        [Key]
        public int id { get; set; }
        public string categoryName { get; set; }
    }
}