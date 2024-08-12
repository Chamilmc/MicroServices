namespace Mongo.Services.AuthAPI.Models.Dto
{
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNUmber { get; set; }
        public string Password { get; set; }

    }
}
