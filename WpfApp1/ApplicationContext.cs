using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    // Класс для взаимодействия с базой данных
    class ApplicationContext : DbContext
    {
        public DbSet<ProductAccepted> ProductsAccepted { get; set; } // Таблица принятых товаров
        public DbSet<ProductStock> ProductsStock { get; set; } // Таблица товаров на складе
        public DbSet<ProductSold> ProductsSold { get; set; } // Таблица проданных товаров
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlServer("Server=WIN-5BA527BT0VC;Database=ProductsDB;Trusted_Connection=True;");
        }
    }
}
