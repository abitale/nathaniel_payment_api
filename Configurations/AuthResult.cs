using System.Collections.Generic;

namespace PaymentApi.Configurations
{
    public class AuthResult
    {
        public string token { get; set; }
        public bool success { get; set; }
        public List<string> errors { get; set; }
    }
}