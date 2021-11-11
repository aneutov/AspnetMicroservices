using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class BasketController : ControllerBase
  {
    private readonly IBasketRepository _repository;
    private readonly DiscountGrpcService _discountService;
    private readonly ILogger<BasketController> _logger;

    public BasketController(IBasketRepository repository, DiscountGrpcService discountService,ILogger<BasketController> logger)
    {
      this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
      this._discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
      this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType((int) HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
    {
      var basket = await _repository.GetBasket(userName);
      return Ok(basket ?? new ShoppingCart(userName));
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
    {

      foreach (var item in basket.Items)
      {
        var discount = await _discountService.GetDiscount(item.ProductName);
        item.Price -= discount.Amount;
      }

      return Ok(await _repository.UpdateBasket(basket));
    }

    [HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteBasket(string userName)
    {
      await _repository.DeleteBasket(userName);
      return NoContent();
    }
  }
}
