using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
  public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
  {
    private readonly IDiscountRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(IDiscountRepository discountRepository, IMapper mapper, ILogger<DiscountService> logger)
    {
      this._repository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
      this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
      var coupon = await _repository.GetDiscount(request.ProductName);

      if (coupon == null)
      {
        throw new RpcException(new Status(StatusCode.NotFound,
          $"Discount with  ProductName={request.ProductName} was not found."));
      }

      _logger.LogInformation($"Discount for the ProductName={request.ProductName} was retrieved succesfully");

      var couponModel = _mapper.Map<CouponModel>(coupon);

      return couponModel;
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
      var coupon = _mapper.Map<Coupon>(request.Coupon);
      await _repository.CreateDiscount(coupon);
      var couponModel = _mapper.Map<CouponModel>(coupon);
      return couponModel;
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
      var coupon = _mapper.Map<Coupon>(request.Coupon);
      await _repository.UpdateDiscount(coupon);
      var couponModel = _mapper.Map<CouponModel>(coupon);
      return couponModel;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
      var result = await _repository.DeleteDiscount(request.ProductName);

      var response = new DeleteDiscountResponse { Success = result };
      return response;
    }
  }
}
