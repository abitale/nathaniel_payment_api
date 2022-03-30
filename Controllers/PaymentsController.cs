using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PaymentApi.Data;
using PaymentApi.Models;

namespace PaymentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var data = await _context.PayData.ToListAsync();
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayments(PaymentData data)
        {
            if (ModelState.IsValid)
            {
                await _context.PayData.AddAsync(data);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAllPayments", new { data.paymentDetailsId }, data);
            }

            return new JsonResult("Something went wrong")
            {
                StatusCode = 500
            };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayments(int id)
        {
            var data = await _context.PayData.FirstOrDefaultAsync(x => x.paymentDetailsId == id);
            if (data == null)
                return NotFound();

            return Ok(data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayments(int id, PaymentData data)
        {
            if (id != data.paymentDetailsId)
                return BadRequest();

            var existData = await _context.PayData.FirstOrDefaultAsync(x => x.paymentDetailsId == id);

            if (existData == null)
                return NotFound();

            existData.cardOwnerName = data.cardOwnerName;
            existData.cardNumber = data.cardNumber;
            existData.expirationDate = data.expirationDate;
            existData.securityCode = data.securityCode;

            await _context.SaveChangesAsync();

            return Ok("Data update success");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentDetails(int id)
        {
            var existData = await _context.PayData.FirstOrDefaultAsync(x => x.paymentDetailsId == id);

            if (existData == null)
                return NotFound();

            _context.PayData.Remove(existData);
            await _context.SaveChangesAsync();

            return Ok(existData);
        }
    }
}