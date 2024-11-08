﻿using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CouponController : Controller
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<IActionResult> CouponIndex()
    {
        List<CouponDto> list = new();

        ResponseDto? response = await _couponService.GetAllCouponAsync();

        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response!.Result!)
                ?? string.Empty)
                ?? new List<CouponDto>();
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(list);
    }

    public IActionResult CouponCreate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CouponCreate(CouponDto couponDto)
    {
        if (ModelState.IsValid)
        {
            ResponseDto? response = await _couponService.CreateCouponAsync(couponDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon create successfully.";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
        }

        return View(couponDto);
    }

    public async Task<IActionResult> CouponDelete(int couponId)
    {
        ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);

        if (response != null && response.IsSuccess)
        {
            var couponDto = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response!.Result!)
               ?? string.Empty)
               ?? new CouponDto();
            return View(couponDto);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CouponDelete(CouponDto couponDto)
    {
        ResponseDto? response = await _couponService.DeleteCouponAsync(couponDto.CouponId);

        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon deleted successfully.";
            return RedirectToAction(nameof(CouponIndex));
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(couponDto);
    }
}
