namespace asp_csharp.Models;

public class PaymentOrder 
{
    public int Id { get; set; }           
    public string StoreName { get; set; }  
    public decimal Amount { get; set; }    
    
    // 🟢 加上問號 ?，代表這個欄位「前端沒傳也沒關係，允許為 Null」
    public string? Status { get; set; } = "處理中"; 
}