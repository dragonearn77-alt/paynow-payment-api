using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp_csharp.Data;
using asp_csharp.Models;

namespace asp_csharp.Controllers;

[ApiController]
[Route("api/[controller]")] // 🎯 自動翻譯：因為叫 PaymentController，所以網址字尾自動變成 /api/payment
public class PaymentController : ControllerBase
{
    private readonly PayNowDbContext _db;

    // 🧙‍♂️ 微軟的相依性注入魔法：開機時會自動車載資料庫經理進來
    public PaymentController(PayNowDbContext db)
    {
        _db = db;
    }

    // 🎯 1. 調閱整本歷史帳本：GET /api/payment/history
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var 歷史紀錄 = await _db.PaymentOrders.ToListAsync();
        return Ok(歷史紀錄);
    }

    // 🎯 2. 精準單筆查詢：GET /api/payment/history/{id}
    [HttpGet("history/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var 訂單 = await _db.PaymentOrders.FindAsync(id);
        if (訂單 == null)
        {
            return NotFound(new { 訊息 = $"查無此交易！找不到單號為 {id} 的訂單。" });
        }
        return Ok(訂單);
    }

    // 🎯 3. 模擬刷卡扣款：POST /api/payment/pay
    [HttpPost("pay")]
    public async Task<IActionResult> Pay([FromBody] PaymentOrder 新訂單)
    {
        if (新訂單.Amount <= 0) 
        {
            return BadRequest("付款失敗：金額必須大於 0 元！");
        }

        新訂單.Status = "付款成功";
        _db.PaymentOrders.Add(新訂單);
        await _db.SaveChangesAsync();

        return Ok(new { 
            訊息 = "PayNow 收到款項了，且已安全記入 MS SQL 資料庫！", 
            單號 = 新訂單.Id, 
            金額 = 新訂單.Amount 
        });
    }
}