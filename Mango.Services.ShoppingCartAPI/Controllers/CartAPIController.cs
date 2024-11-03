using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers;

[Route("api/cart")]
[ApiController]
public class CartAPIController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IProductService _productService;
    private readonly ICouponService _couponService;
    private ResponseDto _response;

    public CartAPIController(AppDbContext appDbContext,
        IMapper mapper,
        IProductService productService,
        ICouponService couponService)
    {
        _response = new ResponseDto();
        _db = appDbContext;
        _mapper = mapper;
        _productService = productService;
        _couponService = couponService;
    }

    [HttpGet("GetCart/{userid}")]
    public async Task<ResponseDto> GetCart(string userid)
    {
        try
        {
            CartDto cart = new()
            {
                CartHeader = _mapper.Map<CartHeaderDto>(await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userid))
            };

            cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(
               _db.CartDetails
                .Where(x => x.CartHeaderId == cart.CartHeader.CartHeaderId));

            IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

            foreach (var item in cart.CartDetails)
            {
                item.Product = productDtos.FirstOrDefault(x => x.ProductId == item.ProductId);
                cart.CartHeader.CartTotal += item.Count * item?.Product?.Price ?? 0;
            }

            if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            {
                CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                {
                    cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                    cart.CartHeader.Discount = coupon.DiscountAmount;
                }
            }

            _response.Result = cart;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpPost("ApplyCoupon")]
    public async Task<ResponseDto> ApplyCoupon(CartDto cartDto)
    {
        try
        {
            var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(x => x.CartHeaderId == cartDto.CartHeader.CartHeaderId);
            cartFromDb!.CouponCode = cartDto.CartHeader.CouponCode;
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpPost("CartUpsert")]
    public async Task<ResponseDto> CartUpsert(CartDto cartDto)
    {
        try
        {
            var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking()
                .FirstOrDefaultAsync(
                x => x.UserId == cartDto.CartHeader.UserId);

            if (cartHeaderFromDb == null)
            {
                var cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                _db.CartHeaders.Add(cartHeader);
                await _db.SaveChangesAsync();

                cartDto.CartDetails!.First().CartHeaderId = cartHeader.CartHeaderId;
                _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails!.First()));

                await _db.SaveChangesAsync();
            }
            else
            {

                var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    x => x.ProductId == cartDto.CartDetails!.First().ProductId &&
                    x.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if (cartDetailsFromDb == null)
                {
                    cartDto.CartDetails!.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails!.First()));
                }
                else
                {
                    cartDto.CartDetails!.First().Count += cartDetailsFromDb.Count;
                    cartDto.CartDetails!.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                    cartDto.CartDetails!.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;

                    _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails!.First()));
                }

                await _db.SaveChangesAsync();
            }

            _response.Result = cartDto;

        }
        catch (Exception ex)
        {
            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpPost("RemoveCart")]
    public async Task<ResponseDto> RemoveCart([FromBody] int cardetailsId)
    {
        try
        {
            var cartDetails = await _db.CartDetails.AsNoTracking()
                .FirstOrDefaultAsync(
                x => x.CartDetailsId == cardetailsId);

            int totalCountOfCartItem = _db.CartDetails.Where(x => x.CartHeaderId == cartDetails!.CartHeaderId).Count();

            _db.CartDetails.Remove(cartDetails!);
            if (totalCountOfCartItem == 1)
            {
                var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(
                    x => x.CartHeaderId == cartDetails!.CartHeaderId);

                _db.CartHeaders.Remove(cartHeaderToRemove!);
            }

            await _db.SaveChangesAsync();

            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }

        return _response;
    }
}
