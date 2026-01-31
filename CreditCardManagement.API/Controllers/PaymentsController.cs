using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Services;

namespace CreditCardManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }
        return userId;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var payment = await _paymentService.CreatePaymentAsync(createDto, userId);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the payment", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var userId = GetCurrentUserId();
            var payments = await _paymentService.GetUserPaymentsAsync(userId, startDate, endDate);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving payments", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var payment = await _paymentService.GetPaymentByIdAsync(id, userId);

            if (payment == null)
            {
                return NotFound(new { message = "Payment not found" });
            }

            return Ok(payment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the payment", error = ex.Message });
        }
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<string>>> GetPaymentTypes()
    {
        try
        {
            var types = await _paymentService.GetAvailablePaymentTypesAsync();
            return Ok(types);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving payment types", error = ex.Message });
        }
    }
}
