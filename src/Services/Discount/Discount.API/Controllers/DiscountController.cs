﻿using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Discount.API.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class DiscountController : ControllerBase
  {
    private readonly IDiscountRepository _repository;
    private readonly ILogger<DiscountController> _logger;

    public DiscountController(IDiscountRepository repository,ILogger<DiscountController> logger)
    {
      this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
      this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{productName}", Name = "GetDiscount")]
    [ProducesResponseType((int) HttpStatusCode.OK)]
    public async Task<ActionResult<Coupon>> GetDiscount(string productName)
    {
      var coupon = await _repository.GetDiscount(productName);
      return Ok(coupon);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Coupon),(int) HttpStatusCode.Created)]
    public async Task<ActionResult<bool>> CreateDiscount([FromBody]Coupon coupon)
    {
      await _repository.CreateDiscount(coupon);
      return CreatedAtRoute("GetDiscount", new {ProductName = coupon.ProductName}, coupon);
    }

    [HttpPut]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> UpdateDiscount([FromBody] Coupon coupon)
    {
      return Ok(await _repository.UpdateDiscount(coupon));
    }

    [HttpDelete("{productName}", Name = "DeleteDiscount")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteDiscount(string productName)
    {
      return Ok(await _repository.DeleteDiscount(productName));
    }


  }
}
