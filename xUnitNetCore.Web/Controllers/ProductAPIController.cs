using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xUnitNetCore.Web.Models;
using xUnitNetCore.Web.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace xUnitNetCore.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IRepository<Product> _repository;

        public ProductAPIController(IRepository<Product> repository)
        {
            _repository = repository;
        }

        [HttpGet("{a}/{b}")]
        public IActionResult Add(int a,int b)
        {
            var result = new Helper.Helper().add(a, b);
            return Ok(result);
        }

        // GET: api/<ProductAPIController>
        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            var products = await _repository.GetAll();
            return Ok(products);
        }

        // GET api/<ProductAPIController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _repository.GetById(id);
            if(product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST api/<ProductAPIController>
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            await _repository.Create(product);
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // PUT api/<ProductAPIController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if(id != product.Id)
            {
                return BadRequest();
            }
            await _repository.Update(product);
            return NoContent();
        }



        // DELETE api/<ProductAPIController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _repository.GetById(id);
            if(product == null)
            {
                return NotFound();
            }
            _repository.Delete(product);
            return NoContent();
        }

        private bool ProductExists(int id)
        {
            Product product = _repository.GetById(id).Result;
            if(product == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
