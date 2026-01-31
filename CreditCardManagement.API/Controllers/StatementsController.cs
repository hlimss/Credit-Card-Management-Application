using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Services;

namespace CreditCardManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatementsController : ControllerBase
{
    private readonly IStatementService _statementService;

    public StatementsController(IStatementService statementService)
    {
        _statementService = statementService;
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
    public async Task<ActionResult<StatementDto>> GenerateStatement([FromBody] CreateStatementDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var statement = await _statementService.GenerateStatementAsync(createDto, userId);
            return CreatedAtAction(nameof(GetStatement), new { id = statement.Id }, statement);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while generating the statement", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StatementDto>>> GetStatements([FromQuery] Guid? creditCardId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var statements = await _statementService.GetUserStatementsAsync(userId, creditCardId);
            return Ok(statements);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving statements", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StatementDto>> GetStatement(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var statement = await _statementService.GetStatementByIdAsync(id, userId);

            if (statement == null)
            {
                return NotFound(new { message = "Statement not found" });
            }

            return Ok(statement);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the statement", error = ex.Message });
        }
    }
}
