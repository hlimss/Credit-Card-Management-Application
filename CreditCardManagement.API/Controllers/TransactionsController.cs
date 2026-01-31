using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Services;

namespace CreditCardManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID");
        }
        return userId;
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] CreateTransactionDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var transaction = await _transactionService.CreateTransactionAsync(createDto, userId);
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the transaction", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);

            if (transaction == null)
            {
                return NotFound(new { message = "Transaction not found" });
            }

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the transaction", error = ex.Message });
        }
    }

    [HttpGet("card/{cardId}")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetCardTransactions(Guid cardId)
    {
        try
        {
            var userId = GetUserId();
            var transactions = await _transactionService.GetTransactionsByCardIdAsync(cardId, userId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving transactions", error = ex.Message });
        }
    }

    [HttpGet("card/{cardId}/expenses")]
    public async Task<ActionResult<CardExpensesDto>> GetCardExpenses(Guid cardId)
    {
        try
        {
            var userId = GetUserId();
            var expenses = await _transactionService.GetCardExpensesAsync(cardId, userId);
            return Ok(expenses);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving card expenses", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactions([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var userId = GetUserId();
            var transactions = await _transactionService.GetAllUserTransactionsAsync(userId, startDate, endDate);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving transactions", error = ex.Message });
        }
    }

    [HttpGet("analytics")]
    public async Task<ActionResult<Dictionary<string, object>>> GetExpenseAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var userId = GetUserId();
            var analytics = await _transactionService.GetExpenseAnalyticsAsync(userId, startDate, endDate);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving analytics", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TransactionDto>> UpdateTransaction(Guid id, [FromBody] UpdateTransactionDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var transaction = await _transactionService.UpdateTransactionAsync(id, updateDto, userId);
            return Ok(transaction);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the transaction", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var deleted = await _transactionService.DeleteTransactionAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new { message = "Transaction not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the transaction", error = ex.Message });
        }
    }
}
