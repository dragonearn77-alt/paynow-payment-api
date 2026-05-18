using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔌 連接 MS SQL 資料庫設定
var connectionString = "Server=db,1433;Database=PayNowDB;User Id=sa;Password=PayNowPassword999;TrustServerCertificate=True;";
builder.Services.AddDbContext<PayNowDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// 🏗️ 自動蓋大樓魔法
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PayNowDbContext>();
    db.Database.EnsureCreated();
}

// 🎯 功能一：調閱「整本」歷史紀錄帳本
app.MapGet("/api/history", async (PayNowDbContext db) => 
{
    var 歷史紀錄 = await db.PaymentOrders.ToListAsync();
    return Results.Ok(歷史紀錄);
});

// ----------------------------------------------------
// 🎯 ✨ 【全新功能】依單號查詢特定某一筆訂單
// 網址裡面的 {id:int} 代表：「這裡會傳進一個整數，請幫我把它抓出來當成變數 id」
// ----------------------------------------------------
app.MapGet("/api/history/{id:int}", async (int id, PayNowDbContext db) => 
{
    // 🧙‍♂️ EF Core 翻譯官再次出動：
    // FindAsync(id) 會被自動翻譯成：「SELECT * FROM PaymentOrders WHERE Id = id;」
    // 它會直接去主鍵（Primary Key）欄位精準搜索
    var 訂單 = await db.PaymentOrders.FindAsync(id);
    
    // 防禦性程式設計：萬一客人亂輸入一個根本不存在的單號（例如 9999）
    if (訂單 == null)
    {
        // 回傳標準的 HTTP Status 404 (Not Found)
        return Results.NotFound(new { 訊息 = $"查無此交易！找不到單號為 {id} 的訂單。" });
    }
    
    // 如果找到了，就只回傳這筆特定訂單的 JSON 給瀏覽器
    return Results.Ok(訂單);
});

// 🎯 功能三：刷卡扣款 (POST)
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