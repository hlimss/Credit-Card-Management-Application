using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Services;

namespace CreditCardManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BankTransfersController : ControllerBase
{
    private readonly IBankTransferService _transferService;

    public BankTransfersController(IBankTransferService transferService)
    {
        _transferService = transferService;
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
    public async Task<ActionResult<BankTransferDto>> CreateTransfer([FromBody] CreateBankTransferDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var transfer = await _transferService.CreateTransferAsync(createDto, userId);
            return CreatedAtAction(nameof(GetTransfer), new { id = transfer.Id }, transfer);
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
            return StatusCode(500, new { message = "An error occurred while creating the transfer", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BankTransferDto>>> GetTransfers([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transfers = await _transferService.GetUserTransfersAsync(userId, startDate, endDate);
            return Ok(transfers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving transfers", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BankTransferDto>> GetTransfer(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transfer = await _transferService.GetTransferByIdAsync(id, userId);

            if (transfer == null)
            {
                return NotFound(new { message = "Transfer not found" });
            }

            return Ok(transfer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the transfer", error = ex.Message });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelTransfer(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var cancelled = await _transferService.CancelTransferAsync(id, userId);

            if (!cancelled)
            {
                return NotFound(new { message = "Transfer not found or cannot be cancelled" });
            }

            return Ok(new { message = "Transfer cancelled successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while cancelling the transfer", error = ex.Message });
        }
    }
}
