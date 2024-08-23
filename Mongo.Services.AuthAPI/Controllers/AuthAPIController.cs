using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mongo.Services.AuthAPI.Models.Dto;
using Mongo.Services.AuthAPI.Service.IService;

namespace Mongo.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto request)
        {
            var responseMessage = await _authService.Register(request);

            if (!string.IsNullOrEmpty(responseMessage))
            {
                _response.IsSuccess = false;
                _response.Message = responseMessage;

                return BadRequest(_response);
            }

            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var loginResponse = await _authService.Login(request);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }

            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto request)
        {
            var assignRoleSucessful = await _authService.AssignRole(request.Email, request?.Role?.ToUpper());
            if (!assignRoleSucessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encounted";
                return BadRequest(_response);
            }

            return Ok(_response);
        }
    }
}
