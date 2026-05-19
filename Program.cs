using Microsoft.EntityFrameworkCore;
using asp_csharp.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. 🔌 註冊資料庫連線字串
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PayNowDbContext>(options => options.UseSqlServer(connectionString));

// 🎯 媒合魔法：當有人要介面（合約），就派發右邊的實體管理員過去
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
// 2. 🏗️ 核心關鍵：告訴 .NET 引擎，我們這次要啟用專業的 Controllers 掃描機制！
builder.Services.AddControllers();

var app = builder.Build();
// 🎯 【加裝全域防護罩】：強迫所有請求與回傳，通通都要經過 ExceptionHandlingMiddleware 的眼線！
app.UseMiddleware<asp_csharp.Middlewares.ExceptionHandlingMiddleware>();

// 3. 🏗️ 自動蓋大樓魔法
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PayNowDbContext>();
    db.Database.EnsureCreated();
}

// 4. 🔀 自動對應路由：它會自己去 Controllers 資料夾把所有寫好的 API 網址拉出來營業
app.MapControllers();

app.Run();