using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class CartService : ICartService
{
    private readonly IBaseService _baseService;

    public CartService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPI + "/api/cart/ApplyCoupon"
        });
    }

    public async Task<ResponseDto?> EmailCart(CartDto cartDto)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPI + "/api/cart/EmailCartRequest"
        });
    }

    public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = SD.ShoppingCartAPI + "/api/cart/GetCart/" + userId
        });
    }

    public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = cartDetailsId,
            Url = SD.ShoppingCartAPI + "/api/cart/RemoveCart"
        });
    }

    public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPI + "/api/cart/CartUpsert"
        });
    }
}
