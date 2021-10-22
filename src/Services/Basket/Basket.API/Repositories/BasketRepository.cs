using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
  public class BasketRepository : IBasketRepository
  {
    private readonly IDistributedCache _redisCache;

    public BasketRepository(IDistributedCache cache)
    {
      this._redisCache = cache ??  throw new ArgumentNullException(nameof(cache));
    }

    public async Task<ShoppingCart> GetBasket(string userName)
    {
      var basketString = await _redisCache.GetStringAsync(userName);
      bool isSomethingInBasket = !String.IsNullOrEmpty(basketString);

      return isSomethingInBasket
        ? JsonSerializer.Deserialize<ShoppingCart>(basketString)
        : null;
    }


    public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
    {
      await _redisCache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));

      return await GetBasket(basket.UserName);
    }

    public async Task DeleteBasket(string userName) => await _redisCache.RemoveAsync(userName);

  }
}
