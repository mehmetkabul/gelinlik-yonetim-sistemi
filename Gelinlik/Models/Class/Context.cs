using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Gelinlik.Models.Class
{
    public class Context : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Rental> Rentals { get; set; } // ✅ Kiralama işlemleri için DbSet
        public DbSet<Sales> Sales { get; set; }
        public DbSet<Appointment> Appointments { get; set; } // ✅ Randevular eklendi
        public DbSet<CustomOrder> CustomOrders { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }

        public DbSet<Setting> SiteSettings { get; set; }

    }

}