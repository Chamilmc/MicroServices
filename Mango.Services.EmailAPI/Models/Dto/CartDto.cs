namespace Mango.Services.EmailAPI.Models.Dto;

public class CartDto
{
    public CartHeaderDto CartHeader { get; set; } = new();
    public IEnumerable<CartDetailsDto>? CartDetails { get; set; }
}
