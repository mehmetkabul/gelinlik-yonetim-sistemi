using System.Collections.Generic;
using System.Data.Entity;

namespace Gelinlik.Models.Class
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; } = new List<Category>(); // Kategoriler listesi
        public Category Category { get; set; } // ✅ Tek kategori ekleme için nesne
        public List<Products> Products { get; set; } = new List<Products>(); // Ürünler listesi
        public Products Product { get; set; } = new Products(); // Tek ürün nesnesi

        public List<Customer> Customers { get; set; } = new List<Customer>(); // ✅ Müşteri listesi
        public Customer Customer { get; set; } = new Customer(); // ✅ Tek müşteri nesnesi

        public List<Rental> Rentals { get; set; } = new List<Rental>(); // ✅ Kira listesi
        public Rental Rental { get; set; } = new Rental(); // ✅ Tek kira nesnesi

        public List<Sales> Sales { get; set; } = new List<Sales>(); // ✅ Satış Listesi
        public Sales Sale { get; set; } = new Sales(); // ✅ Tek Satış nesnesi

        public List<Appointment> Appointments { get; set; } = new List<Appointment>(); // ✅ Satış Listesi
        public Appointment Appointment { get; set; } = new Appointment(); // ✅ Tek Satış nesnesi

        public List<CustomOrder> CustomOrders { get; set; } = new List<CustomOrder>(); // ✅ Özel Dikim Listesi
        public CustomOrder CustomOrder { get; set; } = new CustomOrder(); // ✅ Tek Özel dikim nesnesi

        public List<User> Users { get; set; } = new List<User>(); // ✅ Üye Listesi
        public User User { get; set; } = new User(); // ✅ Tek Üye nesnesi

        public List<Activity> Activities { get; set; } = new List<Activity>(); // ✅ Aktivite Listesi
        public Activity Activity { get; set; } = new Activity(); // ✅ Tek Aktivite nesnesi

        public List<Rental> OverdueRentals { get; set; }
        public List<Rental> DueTomorrowRentals { get; set; }
        public List<Appointment> TodaysAppointments { get; set; }
        public List<Products> LowStockProducts { get; set; }
        public List<Rental> RentalsWithoutBond { get; set; }
        public List<Rental> DueTodayRentals { get; set; }
        public List<CustomOrder> PendingCustomOrders { get; set; }

    }


}