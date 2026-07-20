using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gelinlik.Models.Class
{
    public class Setting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public string LogoPath { get; set; }
    }
}