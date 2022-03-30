using System.ComponentModel.DataAnnotations;

namespace PaymentApi.Models.DTOs.Requests
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}