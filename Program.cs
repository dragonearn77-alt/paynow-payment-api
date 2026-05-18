using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔌 1. 告訴設計師（builder）：我們要加裝 MS SQL 連線工具，並設定好保險箱的通關密碼
// 這串字串叫做 Connection String（連線字串），指明了我們要去 localhost 找 sa 帳號與密碼
var connectionString = "Server=localhost,1433;Database=PayNowDB;User Id=sa;Password=PayNowPassword999;TrustServerCertificate=True;";
builder.Services.AddDbContext<PayNowDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// 🏗️ 2. 自動蓋大樓魔法：程式開機時，EF Core 會去看 MS SQL 裡面有沒有交易表格，沒有的話會自動幫我們蓋好！
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PayNowDbContext>();
    
    db.Database.EnsureCreated(); // 自動在 MS SQL 裡建立對應的資料表
}

// 🎯 當電商網站發送「付款請求 (POST)」到 /api/pay 時執行的邏輯
// 注意：我們在括號裡偷偷塞進了「PayNowDbContext db」，EF Core 就會自動把資料庫鑰匙遞給這段邏輯
app.MapPost("/api/pay", async (PaymentOrder 新訂單, PayNowDbContext db) => 
{
    // 檢查金額
    if (新訂單.Amount <= 0) 
    {
        return Results.BadRequest("付款失敗：金額必須大於 0 元！");
    }

    新訂單.Status = "付款成功";
    
    // 💾 3. 真正的持久化：不再用 List，改把交易直接丟進 MS SQL 的硬碟保險箱！
    db.PaymentOrders.Add(新訂單); // 放入保險箱的暫存推車
    await db.SaveChangesAsync();   // 「碰！」一聲鎖上保險箱硬碟！

    // 回傳給電商網站的結果
    return Results.Ok(new { 
        訊息 = "PayNow 收到款項了，且已安全記入 MS SQL 資料庫！", 
        單號 = 新訂單.Id, // 超神奇：EF Core 寫入成功後，會自動幫我們拿到資料庫生成的全新 ID！
        金額 = 新訂單.Amount 
    });
});

app.Run();

// ==========================================
// 🧙‍♂️ 活頁夾經理（Database Context）：負責 C# 物件與 MS SQL 之間的橋樑
// ==========================================
public class PayNowDbContext : DbContext
{
    public PayNowDbContext(DbContextOptions<PayNowDbContext> options) : base(options) { }
    
    // 這行代表資料庫裡的「資料表 (Table)」，名字叫 PaymentOrders
    public DbSet<PaymentOrder> PaymentOrders { get; set; } 
}

// 定義「訂單」的格式
public class PaymentOrder 
{
    public int Id { get; set; }           // 訂單編號（MS SQL 會自動幫它 1, 2, 3 往下累加）
    public string StoreName { get; set; }  // 商店名稱
    public decimal Amount { get; set; }    // 付款金額
    public string Status { get; set; }     // 狀態
}