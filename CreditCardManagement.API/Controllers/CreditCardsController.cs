using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Services;
using CreditCardManagement.API.Data;

namespace CreditCardManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CreditCardsController : ControllerBase
{
    private readonly ICreditCardService _creditCardService;
    private readonly IWhatsAppService _whatsAppService;
    private readonly ApplicationDbContext _context;

    public CreditCardsController(
        ICreditCardService creditCardService,
        IWhatsAppService whatsAppService,
        ApplicationDbContext context)
    {
        _creditCardService = creditCardService;
        _whatsAppService = whatsAppService;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CreditCardDto>>> GetCreditCards()
    {
        var userId = GetCurrentUserId();
        var cards = await _creditCardService.GetUserCreditCardsAsync(userId);
        return Ok(cards);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CreditCardDto>> GetCreditCard(Guid id)
    {
        var userId = GetCurrentUserId();
        var card = await _creditCardService.GetCreditCardByIdAsync(id, userId);

        if (card == null)
        {
            return NotFound(new { message = "Credit card not found" });
        }

        return Ok(card);
    }

    [HttpPost]
    public async Task<ActionResult<CreditCardDto>> CreateCreditCard([FromBody] CreateCreditCardDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            var card = await _creditCardService.CreateCreditCardAsync(createDto, userId);
            
            // Send WhatsApp notification
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.PhoneNumber))
                {
                    await _whatsAppService.SendCardAddedMessageAsync(
                        user.PhoneNumber,
                        card.CardholderName,
                        card.CardType,
                        card.Balance
                    );
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail the card creation if WhatsApp fails
                // In production, you might want to use a background job for this
                Console.WriteLine($"Failed to send WhatsApp notification: {ex.Message}");
            }
            
            return CreatedAtAction(nameof(GetCreditCard), new { id = card.Id }, card);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the full exception for debugging
            return StatusCode(500, new { message = "An error occurred while creating the credit card", error = ex.Message, details = ex.InnerException?.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CreditCardDto>> UpdateCreditCard(Guid id, [FromBody] UpdateCreditCardDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            var card = await _creditCardService.UpdateCreditCardAsync(id, updateDto, userId);
            return Ok(card);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the credit card", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCreditCard(Guid id)
    {
        var userId = GetCurrentUserId();
        var deleted = await _creditCardService.DeleteCreditCardAsync(id, userId);

        if (!deleted)
        {
            return NotFound(new { message = "Credit card not found" });
        }

        return NoContent();
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
}

