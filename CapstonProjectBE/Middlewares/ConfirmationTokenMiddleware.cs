using Application;

namespace CapstonProjectBE.Middlewares
{
    public class ConfirmationTokenMiddleware
    {
        private readonly RequestDelegate _next;
        public ConfirmationTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // create area service temporary
            using (var scope = context.RequestServices.CreateScope())
            {
                // Get the IUnitOfWork from the temporary service scope
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var tokenValue = context.Request.Query["token"];

                if (!string.IsNullOrEmpty(tokenValue))
                {
                    var token = await unitOfWork.TokenRepo.GetTokenWithUser(tokenValue, "confirmation");

                    if (token != null && token.User != null)
                    {
                        if (DateTime.UtcNow.AddHours(7) > token.ExpiresAt)
                        {
                            await context.Response.WriteAsync("Token xác nhận đã hết hạn. Vui lòng yêu cầu gửi lại email xác nhận.");
                            return;
                        }
                        token.TokenValue = "success";

                        await unitOfWork.SaveChangeAsync();
                        context.Response.Redirect("http://game-mkt.vercel.app/login");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
