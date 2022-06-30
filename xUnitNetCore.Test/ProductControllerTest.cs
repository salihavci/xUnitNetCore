using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xUnitNetCore.Web.Models;

namespace xUnitNetCore.Test
{
    public class ProductControllerTest
    {
        protected DbContextOptions<AppDbContext> _contextOptions { get; private set; }
        public void SetContextOptions(DbContextOptions<AppDbContext> contextOptions)
        {
            _contextOptions = contextOptions;
            Seed();
        }
        public void Seed()
        {
            using (AppDbContext context = new AppDbContext(_contextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Category.Add(new Category { Name = "Kalemler" });
                context.Category.Add(new Category { Name = "Defterler" });
                context.SaveChanges();
                context.Products.Add(new Product { CategoryId = 1 ,Name = "Kalem 10",Color = "Black",Price = 100,Stock = 200});
                context.Products.Add(new Product { CategoryId = 1 ,Name = "Kalem 20",Color = "Gray",Price = 200,Stock = 100});
                context.SaveChanges();
            }
        }
    }
}
