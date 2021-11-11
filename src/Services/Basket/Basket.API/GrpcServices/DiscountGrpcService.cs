using Discount.Grpc.Protos;
using System;
using System.Threading.Tasks;

namespace Basket.API.GrpcServices
{
  public class DiscountGrpcService
  {
    private readonly DiscountProtoService.DiscountProtoServiceClient _discountGrpcService;

    public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountGrpcService)
    {
      _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
    }

    public async Task<CouponModel> GetDiscount(string productName)
    {
      var discountRequest = new GetDiscountRequest { ProductName = productName };

      return await _discountGrpcService.GetDiscountAsync(discountRequest);
    }
  }
}
