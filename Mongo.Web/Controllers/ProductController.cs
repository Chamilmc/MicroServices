﻿using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> ProductIndex()
    {
        List<ProductDto> list = new();

        ResponseDto? response = await _productService.GetAllProductAsync();

        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response!.Result!)
            ?? string.Empty)
            ?? new List<ProductDto>();
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(list);
    }

    public IActionResult ProductCreate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProductCreate(ProductDto productDto)
    {
        if (ModelState.IsValid)
        {
            ResponseDto? response = await _productService.CreateProductAsync(productDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product create successfully.";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
        }

        return View(productDto);
    }

    public async Task<IActionResult> ProductDelete(int productId)
    {
        ResponseDto? response = await _productService.GetProductByIdAsync(productId);

        if (response != null && response.IsSuccess)
        {                
            var productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response!.Result!)
            ?? string.Empty)
            ?? new ProductDto();
            return View(productDto);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductDelete(ProductDto productDto)
    {
        ResponseDto? response = await _productService.DeleteProductAsync(productDto.ProductId);

        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Product deleted successfully.";
            return RedirectToAction(nameof(ProductIndex));
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(productDto);
    }

    public async Task<IActionResult> ProductEdit(int productId)
    {
        ResponseDto? response = await _productService.GetProductByIdAsync(productId);

        if (response != null && response.IsSuccess)
        {               
            var productto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response!.Result!)
            ?? string.Empty)
            ?? new ProductDto();
            return View(productto);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductEdit(ProductDto productDto)
    {
        ResponseDto? response = await _productService.UpdateProductAsync(productDto);

        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Product update successfully.";
            return RedirectToAction(nameof(ProductIndex));
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(productDto);
    }
}
