using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using xUnitNetCore.Web.Controllers;
using xUnitNetCore.Web.Models;

namespace xUnitNetCore.Test
{
    public class ProductControllerTestWithInMemory:ProductControllerTest
    {
        public ProductControllerTestWithInMemory()
        {
            SetContextOptions(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("UdemyUnitTestInMemoryDb").Options);
            
        }

        [Fact]
        public async Task Create_ModelValidProduct_ReturnRedirectToActionWithSaveProduct()
        {

            var newProduct = new Product() { Name = "Kalem 30", Price = 200, Stock = 200};
            using (var context = new AppDbContext(_contextOptions))
            {
                var category = context.Category.First();
                newProduct.CategoryId = category.Id;
                var controller = new ProductsController(context);
                var result = await controller.Create(newProduct);
                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirect.ActionName);
            }

            using (var context = new AppDbContext(_contextOptions))
            {
                var product = context.Products.FirstOrDefault(x => x.Name == "Kalem 30");
                Assert.Equal(newProduct.Name, product.Name);
            }
            
        }

        [Theory]
        [InlineData(1)]
        public async Task DeleteCategory_ExistCategoryId_DeleteAllProducts(int categoryId)
        {
            using (var context = new AppDbContext(_contextOptions))
            {
                var category = await context.Category.FindAsync(categoryId); //Primary key'e göre arama yapar
                context.Category.Remove(category);
                context.SaveChanges();
            }
            using (var context = new AppDbContext(_contextOptions))
            {
                var products = await context.Products.Where(x => x.CategoryId == categoryId).ToListAsync();
                Assert.Empty(products);
            }
        }
    }
}
