using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔌 連接 MS SQL 資料庫設定
var connectionString = "Server=localhost,1433;Database=PayNowDB;User Id=sa;Password=PayNowPassword999;TrustServerCertificate=True;";
builder.Services.AddDbContext<PayNowDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// 🏗️ 自動蓋大樓魔法
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PayNowDbContext>();
    db.Database.EnsureCreated();
}

// ----------------------------------------------------
// 🎯 ✨ 【全新功能】加裝 GET API：調閱歷史紀錄帳本
// ----------------------------------------------------
app.MapGet("/api/history", async (PayNowDbContext db) => 
{
    // 🧙‍♂️ EF Core 翻譯官出動：
    // 這行 C# 會被自動翻譯成 SQL 語法：「SELECT * FROM PaymentOrders;」
    // 它會去資料庫把整張表格挖出來，並變成一個 C# 的清單（List）
    var 歷史紀錄 = await db.PaymentOrders.ToListAsync();
    
    // 把整本帳本轉成 JSON 格式回傳給瀏覽器
    return Results.Ok(歷史紀錄);
});

// 🎯 POST API：刷卡扣款
app.MapPost("/api/pay", async (PaymentOrder 新訂單, PayNowDbContext db) => 
{
    if (新訂單.Amount <= 0) 
    {
        return Results.BadRequest("付款失敗：金額必須大於 0 元！");
    }

    新訂單.Status = "付款成功";
    db.PaymentOrders.Add(新訂單);
    await db.SaveChangesAsync();

    return Results.Ok(new { 
        訊息 = "PayNow 收到款項了，且已安全記入 MS SQL 資料庫！", 
        單號 = 新訂單.Id, 
        金額 = 新訂單.Amount 
    });
});

app.Run();

// ==========================================
// 活頁夾經理與資料結構（不用變動）
// ==========================================
public class PayNowDbContext : DbContext
{
    public PayNowDbContext(DbContextOptions<PayNowDbContext> options) : base(options) { }
    public DbSet<PaymentOrder> PaymentOrders { get; set; } 
}

public class PaymentOrder 
{
    public int Id { get; set; }           
    public string StoreName { get; set; }  
    public decimal Amount { get; set; }    
    public string Status { get; set; }     
}