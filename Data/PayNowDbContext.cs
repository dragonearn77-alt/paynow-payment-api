using Microsoft.EntityFrameworkCore;
using asp_csharp.Models;

namespace asp_csharp.Data;

public class PayNowDbContext : DbContext
{
    public PayNowDbContext(DbContextOptions<PayNowDbContext> options) : base(options) { }
    public DbSet<PaymentOrder> PaymentOrders { get; set; } 
}