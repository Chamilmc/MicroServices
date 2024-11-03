using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers;

[Route("api/product")]
[ApiController]
public class ProductAPIController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    public ProductAPIController(AppDbContext appDbContext,
        IMapper mapper)
    {
        _db = appDbContext;
        _mapper = mapper;
        _response = new ResponseDto();
    }

    [HttpGet]
    public ResponseDto Get()
    {
        try
        {
            IEnumerable<Product> objList = _db.Products.ToList();
            _response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }

        return _response;
    }

    [HttpGet]
    [Route("{id:int}")]
    public ResponseDto Get(int id)
    {
        try
        {
            _response.Result = _mapper.Map<ProductDto>(_db.Products.FirstOrDefault(x => x.ProductId == id));
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }

        return _response;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public ResponseDto Post([FromBody] ProductDto productDto)
    {
        try
        {
            var obj = _mapper.Map<Product>(productDto);
            _db.Products.Add(obj);
            _db.SaveChanges();

            _response.Result = _mapper.Map<ProductDto>(obj);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }

        return _response;
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public ResponseDto Put([FromBody] ProductDto productDto)
    {
        try
        {
            var obj = _mapper.Map<Product>(productDto);
            _db.Products.Update(obj);

            _db.SaveChanges();

            _response.Result = _mapper.Map<ProductDto>(obj);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }

        return _response;
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public ResponseDto Delete(int id)
    {
        try
        {
            var obj = _db.Products.FirstOrDefault(x => x.ProductId == id);
            _db.Products.Remove(obj!);

            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }

        return _response;
    }
}
