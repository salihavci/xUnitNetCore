using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using xUnitNetCore.Web.Controllers;
using xUnitNetCore.Web.Helper;
using xUnitNetCore.Web.Models;
using xUnitNetCore.Web.Repository;

namespace xUnitNetCore.Test
{
    public class ProductAPIControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductAPIController _controller;
        private List<Product> _products;
        private readonly Helper _helper;
        public ProductAPIControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>(); //MockBehaviour.birşey (Buradaki birşey ENUM değeridir. Testin zorunluluk ve ihmal durumunu belirler.) Katıysa Strict, Gevşek bir test istenilirse'de Loose kullanılır.
            _controller = new ProductAPIController(_mockRepo.Object);
            _products = new List<Product> { new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" }, new Product { Id = 2, Name = "Defter", Price = 200, Stock = 500, Color = "Mavi" } };
            _helper = new Helper();
        }

        [Theory]
        [InlineData(4,5,9)]
        public void Add_SampleValues_ReturnTotal(int a, int b,int total)
        {
            var result = _helper.add(a, b);
            Assert.Equal(total, result);
        }



        [Fact]
        public async void GetProduct_ActionExecute_ReturnOkResultWithProduct()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(_products);
            var result = await _controller.GetProduct();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal<int>(2, returnProduct.ToList().Count());
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInvalid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x=> x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.GetProduct(productId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdValid_ReturnOkResult(int productId)
        {
            Product product = _products.First(m => m.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.GetProduct(productId);
            var okResult = Assert.IsType<OkObjectResult>(result); //Geri dönüşte bir Products yoksa OkResult kontrolü sağlanır.
            var returnProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(productId, returnProduct.Id);
            Assert.Equal(product.Name, returnProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        public async void PutProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = _products.First(m => m.Id == productId);
            var result = await _controller.PutProduct(2, product);
            Assert.IsType<BadRequestResult>(result);
        }
        [Theory]
        [InlineData(1)]
        public async void PutProduct_ActionExecute_ReturnNoContentResult(int productId)
        {
            var product = _products.First(m => m.Id == productId);
            _mockRepo.Setup(repo => repo.Update(product));
            var result = await _controller.PutProduct(productId, product);
            _mockRepo.Verify(repo => repo.Update(product),Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void PostProduct_ActionExecute_ReturnCreatedAtActionResult()
        {
            var product = _products.First();
            _mockRepo.Setup(repo => repo.Create(product)).Returns(Task.CompletedTask); //Asenkron metod dönüşünün asenkron tamamlama sonucu olduğunu belirttik
            var result = await _controller.PostProduct(product);
            var createdAtAction = Assert.IsType<CreatedAtActionResult>(result);
            _mockRepo.Verify(x=> x.Create(product), Times.Once);
            Assert.Equal("GetProduct", createdAtAction.ActionName);
        }

        [Theory]
        [InlineData(500)]
        public async void DeleteProduct_IdInvalid_ReturnNotFoundResult(int productId)
        {
            Product product = null;
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.DeleteProduct(productId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecute_ReturnNoContent(int productId)
        {
            var product = _products.First(m => m.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            _mockRepo.Setup(repo => repo.Delete(product));
            var result = await _controller.DeleteProduct(productId);
            _mockRepo.Verify(x => x.Delete(product), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
