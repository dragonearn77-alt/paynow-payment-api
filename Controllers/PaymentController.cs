using Microsoft.AspNetCore.Mvc;
using asp_csharp.Data;
using asp_csharp.Models;

namespace asp_csharp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    // 🎯 核心改變：Controller 從此不再認識任何實體資料庫，它只認合約 (介面)！
    private readonly IPaymentRepository _repo;

    public PaymentController(IPaymentRepository repo)
    {
        _repo = repo;
    }

    // 🎯 1. 調閱整本歷史帳本
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        // 💣 🎯 【惡意模擬炸彈】：假裝突然發生了驚天動地的連線崩潰！
    throw new Exception("MS SQL 資料庫被駭客拔掉插頭，連線徹底中斷！");

        var 歷史紀錄 = await _repo.GetAllOrdersAsync();
        return Ok(歷史紀錄);
    }

    // 🎯 2. 精準單筆查詢
    [HttpGet("history/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var 訂單 = await _repo.GetOrderByIdAsync(id);
        if (訂單 == null)
        {
            return NotFound(new { 訊息 = $"查無此交易！找不到單號為 {id} 的訂單。" });
        }
        return Ok(訂單);
    }

    // 🎯 3. 模擬刷卡扣款
    [HttpPost("pay")]
    public async Task<IActionResult> Pay([FromBody] PaymentOrder 新訂單)
    {
        if (新訂單.Amount <= 0) 
        {
            return BadRequest("付款失敗：金額必須大於 0 元！");
        }

        新訂單.Status = "付款成功";
        
        // 吩咐管理員去存檔，Controller 根本不知道也不想管他是怎麼存進去的
        await _repo.AddOrderAsync(新訂單);

        return Ok(new { 
            訊息 = "PayNow 收到款項了，系統已透過微服務架構安全存檔！", 
            單號 = 新訂單.Id, 
            金額 = 新訂單.Amount 
        });
    }
}