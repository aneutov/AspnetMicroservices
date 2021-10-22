using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class CatalogController : ControllerBase
  {
    private readonly IProductRepository _repository;
    private readonly ILogger _logger;

    public CatalogController(IProductRepository repository, ILogger<CatalogController> logger )
    {
      this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
      this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts() => Ok(await _repository.GetProducts());

    [HttpGet("{id:length(24)}", Name = "GetProductById")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<Product>> GetProductById(string id)
    {
      var product = await _repository.GetProductById(id);

      if(product == null)
      {
        _logger.LogError($"Product with id: {id} wasn't found");
        return NotFound();
      }
       return Ok(product);
    }

    [Route("[action]/{category}", Name = "GetProductsByCategory")]
    [HttpGet]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> GetProductsByCategory(string category)
    {
      var products = await _repository.GetProductsByCategory(category);
      return Ok(products);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
      await _repository.CreateProduct(product);

      return CreatedAtRoute("GetProduct", new { id = product.Id},product);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProduct([FromBody] Product product)
    {
      return Ok(await _repository.UpdateProduct(product));
    }

    [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<Product>> DeleteProduct(string id)
    {
      return Ok(await _repository.DeleteProduct(id));
    }

  }
}
