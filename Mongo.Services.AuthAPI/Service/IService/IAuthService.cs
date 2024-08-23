using Mongo.Services.AuthAPI.Models.Dto;

namespace Mongo.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string? eamil, string? roleName);
    }
}
