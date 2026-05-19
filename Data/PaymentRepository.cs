using Microsoft.EntityFrameworkCore;
using asp_csharp.Models;

namespace asp_csharp.Data;

// : IPaymentRepository 代表這個管理員保證會百分之百履行合約規定
public class PaymentRepository : IPaymentRepository
{
    private readonly PayNowDbContext _db;

    public PaymentRepository(PayNowDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<PaymentOrder>> GetAllOrdersAsync()
    {
        return await _db.PaymentOrders.ToListAsync();
    }

    public async Task<PaymentOrder?> GetOrderByIdAsync(int id)
    {
        return await _db.PaymentOrders.FindAsync(id);
    }

    public async Task AddOrderAsync(PaymentOrder order)
    {
        _db.PaymentOrders.Add(order);
        await _db.SaveChangesAsync(); // 真正寫入 MS SQL 
    }
}