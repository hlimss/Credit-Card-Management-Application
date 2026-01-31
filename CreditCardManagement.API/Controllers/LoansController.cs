using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Services;

namespace CreditCardManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
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
    public async Task<ActionResult<LoanDto>> CreateLoan([FromBody] CreateLoanDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var loan = await _loanService.CreateLoanAsync(createDto, userId);
            return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the loan", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
    {
        try
        {
            var userId = GetCurrentUserId();
            var loans = await _loanService.GetUserLoansAsync(userId);
            return Ok(loans);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving loans", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanDto>> GetLoan(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var loan = await _loanService.GetLoanByIdAsync(id, userId);

            if (loan == null)
            {
                return NotFound(new { message = "Loan not found" });
            }

            return Ok(loan);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the loan", error = ex.Message });
        }
    }

    [HttpPost("{id}/payment")]
    public async Task<ActionResult<LoanDto>> MakePayment(Guid id, [FromBody] decimal amount)
    {
        try
        {
            var userId = GetCurrentUserId();
            var loan = await _loanService.MakePaymentAsync(id, amount, userId);
            return Ok(loan);
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
            return StatusCode(500, new { message = "An error occurred while processing the payment", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoan(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var deleted = await _loanService.DeleteLoanAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new { message = "Loan not found" });
            }

            return Ok(new { message = "Loan deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the loan", error = ex.Message });
        }
    }
}
