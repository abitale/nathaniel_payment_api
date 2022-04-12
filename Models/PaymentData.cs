using System.ComponentModel.DataAnnotations;
using System;

namespace PaymentApi.Models
{
    public class PaymentData
    {
        [Key]
        public int paymentDetailsId { get; set; }
        public string cardOwnerName { get; set; }
        public string cardNumber { get; set; }
        public DateTime expirationDate { get; set; }
        public string securityCode { get; set; }
    }
}