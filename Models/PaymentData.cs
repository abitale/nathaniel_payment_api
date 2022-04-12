using System.ComponentModel.DataAnnotations;

namespace PaymentApi.Models
{
    public class PaymentData
    {
        [Key]
        public int paymentDetailsId { get; set; }
        public string cardOwnerName { get; set; }
        public string cardNumber { get; set; }
        public string expirationDate { get; set; }
        public string securityCode { get; set; }
    }
}