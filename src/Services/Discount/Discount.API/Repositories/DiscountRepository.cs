using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
  public class DiscountRepository : IDiscountRepository
  {
    private readonly IConfiguration _configuration;

    public DiscountRepository(IConfiguration configuration)
    {
      this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<Coupon> GetDiscount(string productName)
    {
      var connection = new NpgsqlConnection(_configuration.GetValue<string>("NpgConnectionString"));
      var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
        $"SELECT * FROM Coupon WHERE ProductName = @ProductName",
        new {ProductName = productName}
        );

      if (coupon == null)
      {
        return new Coupon {ProductName = "No Discount", Amount = 0, Description = "No Discount" };
      }

      return coupon;
    }

    public async Task<bool> CreateDiscount(Coupon coupon)
    {
      var connection = new NpgsqlConnection(_configuration.GetValue<string>("NpgConnectionString"));
      var affected = await connection.ExecuteAsync(
        $"INSERT INTO Coupon (ProductName, Description, Amount) values (@ProductName, @Description, @Amount)",
        new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

      return (affected > 0);
    }

    public async Task<bool> UpdateDiscount(Coupon coupon)
    {
      var connection = new NpgsqlConnection(_configuration.GetValue<string>("NpgConnectionString"));
      var affected = await connection.ExecuteAsync(
        $"UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
        new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

      return (affected > 0);
    }
    public async Task<bool> DeleteDiscount(string productName)
    {
      var connection = new NpgsqlConnection(_configuration.GetValue<string>("NpgConnectionString"));
      var affected = await connection.ExecuteAsync(
        $"DELETE from Coupon WHERE ProductName = @ProductName",
        new { ProductName = productName });

      return (affected > 0);
    }

  }
}
