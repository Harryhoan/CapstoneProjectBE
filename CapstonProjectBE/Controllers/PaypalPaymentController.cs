using Application.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstonProjectBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaypalPaymentController : ControllerBase
    {
        private readonly IPaypalPaymentService _paypalPaymentService;
        private readonly IAuthenService _authenService;
        public PaypalPaymentController(IPaypalPaymentService paypalPaymentService, IAuthenService authenService)
        {
            _paypalPaymentService = paypalPaymentService;
            _authenService = authenService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreatePayment(int projectId, decimal amount)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _paypalPaymentService.CreatePaymentAsync(user.UserId, projectId, amount, "http://localhost:50875/payment", "http://localhost:50875/user/cart");

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("execute")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> ExecutePayment([FromQuery] string paymentId, [FromQuery] string token, [FromQuery] string PayerID)
        {
            var result = await _paypalPaymentService.ExecutePaymentAsync(paymentId, PayerID);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("Refund")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateRefundAsync(int pledgeId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _paypalPaymentService.CreateRefundAsync(user.UserId, pledgeId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("TransferPledgeToCreator")]
        [Authorize(Roles = "STAFF, ADMIN")]
        public async Task<IActionResult> TransferPledgeToCreatorAsync(int projectId)
        {
            var user = await _authenService.GetUserByTokenAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _paypalPaymentService.TransferPledgeToCreatorAsync(user.UserId, projectId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("RefundAllPledgesForProject")]
        [Authorize(Roles = "STAFF, ADMIN")]
        public async Task<IActionResult> RefundAllPledgesForProjectAsync(int projectId)
        {
            var result = await _paypalPaymentService.RefundAllPledgesForProjectAsync(projectId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
