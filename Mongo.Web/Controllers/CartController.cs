﻿using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDtoBaseOnLoggedInUser());
    }

    public async Task<IActionResult> Remove(int cartDetailsId)
    {
        var userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        var response = await _cartService.RemoveFromCartAsync(cartDetailsId);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Cart update successfully";
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        var response = await _cartService.ApplyCouponAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon apply successfully";
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> EmailCart()
    {
        CartDto cart= await LoadCartDtoBaseOnLoggedInUser();
        cart.CartHeader.Email = User.Claims
            .Where(x => x.Type == JwtRegisteredClaimNames.Email)?
            .FirstOrDefault()?
            .Value;

        var response = await _cartService.EmailCart(cart);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Email will be processed annd sent shortly.";
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        cartDto.CartHeader.CouponCode = "";
        var response = await _cartService.ApplyCouponAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon remove successfully";
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    private async Task<CartDto> LoadCartDtoBaseOnLoggedInUser()
    {
        var userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        var response = await _cartService.GetCartByUserIdAsync(userId!);
        if (response != null && response.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response!.Result!)
             ?? string.Empty)
                ?? new CartDto();
        }

        return new CartDto();
    }
}
