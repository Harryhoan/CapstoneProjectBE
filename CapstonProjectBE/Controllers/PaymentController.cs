using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        //private readonly IPaymentService _paymentService;
        //private readonly IAuthenService _authenService;

        //public PaymentController(IPaymentService paymentService, IAuthenService authenService)
        //{
        //    _paymentService = paymentService;
        //    _authenService = authenService;
        //}

        ///// <summary>
        ///// Tạo url thanh toán
        ///// </summary>
        ///// <param name="money">Số tiền phải thanh toán</param>
        ///// <param name="description">Mô tả giao dịch</param>
        ///// <returns></returns>
        //[Authorize(Roles = "Customer")]
        //[HttpGet("CreatePaymentUrl")]
        //public async Task<IActionResult> CreatePaymentUrl(double money, string description,[FromQuery] int projectId)
        //{
        //    var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
        //    if (user == null)
        //    {
        //        return Unauthorized();
        //    }
        //    try
        //    {
        //        var paymentUrl = await _paymentService.CreatePaymentUrl(money, description, HttpContext, projectId, user.UserId);
        //        return Created(paymentUrl, paymentUrl);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Thực hiện hành động sau khi thanh toán. URL này cần được khai báo với VNPAY để API này hoạt đồng (ví dụ: http://localhost:1234/api/Vnpay/IpnAction)
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("IpnAction")]
        //public IActionResult IpnAction()
        //{
        //    if (Request.QueryString.HasValue)
        //    {
        //        try
        //        {
        //            var paymentResult = _paymentService.ProcessPaymentResult(Request.Query);
        //            if (paymentResult.IsSuccess)
        //            {
        //                // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
        //                return Ok();
        //            }

        //            // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
        //            return BadRequest("Thanh toán thất bại");
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //    return NotFound("Không tìm thấy thông tin thanh toán.");
        //}

        ///// <summary>
        ///// Trả kết quả thanh toán về cho người dùng
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("Callback")]
        //public ActionResult<PaymentResult> Callback()
        //{
        //    if (Request.QueryString.HasValue)
        //    {
        //        try
        //        {
        //            var paymentResult = _paymentService.ProcessPaymentResult(Request.Query);

        //            if (paymentResult.IsSuccess)
        //            {
        //                return Ok(paymentResult);
        //            }

        //            return BadRequest(paymentResult);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //    return NotFound("Không tìm thấy thông tin thanh toán.");
        //}
    }
}
