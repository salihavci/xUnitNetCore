using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using xUnitNetCore.Web.Controllers;
using xUnitNetCore.Web.Models;
using xUnitNetCore.Web.Repository;

namespace xUnitNetCore.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> _products;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>(); //MockBehaviour.birşey (Buradaki birşey ENUM değeridir. Testin zorunluluk ve ihmal durumunu belirler.) Katıysa Strict, Gevşek bir test istenilirse'de Loose kullanılır.
            _controller = new ProductsController(_mockRepo.Object);
            _products = new List<Product> { new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" }, new Product { Id = 2, Name = "Defter", Price = 200, Stock = 500, Color = "Mavi" } };
        }
        
        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result); //Bu bir view mi değil mi bu test ediliyor.
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(_products);

            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            Assert.Equal<int>(2, productList.Count());
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_IsInvalid_ReturnNotFound()
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);
            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            Product product = _products.First(m => m.Id == productId);
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Details(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal<int>(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePost_InvalidModelState_ReturnViewWithData()
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir.");
            var result = await _controller.Create(_products.First());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(_products.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePost_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;
            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x=> newProduct = x);
            var result = await _controller.Create(_products.First());
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()),Times.Once); //Create methodu en az 1 kere çalışsın diye test ediyoruz.
            Assert.Equal(_products.First().Id,newProduct.Id);
        }

        [Fact]
        public async void CreatePost_InvalidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir.");
            var result = await _controller.Create(_products.First());
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never); //Create methodu çalışmasın diye test ediyoruz.
        }

        [Fact]
        public async void Edit_IdIsNull_RedirectToActionIndex()
        {
            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(int.MaxValue)]
        public async void Edit_IdInvalid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecute_ReturnProduct(int productId)
        {
            Product product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPost_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = await _controller.Edit(2,_products.First(x=> x.Id == productId));
            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPost_InvalidModelState_ReturnView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "Name alanı zorunludur.");
            var result = await _controller.Edit(productId,_products.First(m=> m.Id == productId));
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPost_ValidModelState_ReturnRedirectToIndex(int productId)
        {
            var result = await _controller.Edit(productId, _products.First(x=> x.Id == productId));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPost_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product = _products.First(m => m.Id == productId);
            _mockRepo.Setup(repo => repo.Update(product));
            await _controller.Edit(productId,product);
            _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once());
        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }
        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecute_ReturnProduct(int productId)
        {
            var product = _products.First(m => m.Id == productId);
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Delete(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecute_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.DeleteConfirmed(productId);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecute_DeleteMethodExecute(int productId)
        {
            var product = _products.First(m => m.Id == productId);
            _mockRepo.Setup(repo => repo.Delete(product));
            await _controller.DeleteConfirmed(productId);
            _mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once());
        }
    }
}
