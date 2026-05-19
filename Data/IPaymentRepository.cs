using asp_csharp.Models;

namespace asp_csharp.Data;

public interface IPaymentRepository
{
    // 合約規定 1：必須提供「撈取整本帳本」的功能
    Task<IEnumerable<PaymentOrder>> GetAllOrdersAsync();

    // 合約規定 2：必須提供「依 Id 精準查詢」的功能
    Task<PaymentOrder?> GetOrderByIdAsync(int id);

    // 合約規定 3：必須提供「新增訂單」的功能
    Task AddOrderAsync(PaymentOrder order);
}