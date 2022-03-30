using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PaymentApi.Models;

namespace PaymentApi.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public virtual DbSet<PaymentData> PayData { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}