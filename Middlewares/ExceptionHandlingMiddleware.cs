using System.Net;
using System.Text.Json;

namespace asp_csharp.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // 🛡️ 當客人的請求經過大門時，會觸發這個方法
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // 讓請求繼續往前走，去執行 Controller 的邏輯
            await _next(context);
        }
        catch (Exception ex)
        {
            // 🧙‍♂️ 捕獸夾啟動！如果後面任何一個地方（Controller/Repository）大崩潰，通通在這裡被抱住！
            _logger.LogError($"[⚠️ 系統緊急警報]: 捕獲到未處理的崩潰！錯誤原因: {ex.Message}");
            
            // 轉身去處理，包裝成優雅的回覆給客人
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        // 強制把原本會讓伺服器死機的錯誤，定義為標準的 HTTP 500 (伺服器內部錯誤)
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // 🟢 【關鍵資安防禦】：後台日誌印出真正的錯誤，但吐給前端客人的，必須是絕對安全、看不出規格的官方客製化 JSON！
        var 安全的錯誤回覆 = new
        {
            狀態碼 = context.Response.StatusCode,
            錯誤訊息 = "金流保險箱目前處於維護或過載狀態，請稍後再試。我們已通知工程團隊前往排查！",
            錯誤時間 = DateTime.UtcNow
        };

        var json結果 = JsonSerializer.Serialize(安全的錯誤回覆);
        return context.Response.WriteAsync(json結果);
    }
}