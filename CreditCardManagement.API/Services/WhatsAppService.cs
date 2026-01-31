using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CreditCardManagement.API.Services;

public class WhatsAppService : IWhatsAppService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WhatsAppService> _logger;
    private readonly string? _accountSid;
    private readonly string? _authToken;
    private readonly string? _whatsAppFromNumber;

    public WhatsAppService(IConfiguration configuration, ILogger<WhatsAppService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _accountSid = _configuration["Twilio:AccountSid"];
        _authToken = _configuration["Twilio:AuthToken"];
        _whatsAppFromNumber = _configuration["Twilio:WhatsAppFromNumber"];

        // Initialize Twilio client if credentials are provided
        if (!string.IsNullOrEmpty(_accountSid) && !string.IsNullOrEmpty(_authToken))
        {
            TwilioClient.Init(_accountSid, _authToken);
        }
    }

    public async Task<bool> SendCardAddedMessageAsync(string phoneNumber, string cardholderName, string cardType, decimal? balance)
    {
        var balanceText = balance.HasValue 
            ? $"Le montant exact de {balance.Value:N2} {GetCurrencySymbol()} a été ajouté"
            : "La carte a été ajoutée";

        var message = $"🎉 Bonjour {cardholderName},\n\n" +
                     $"✅ {balanceText} dans votre carte {cardType}.\n\n" +
                     $"Merci d'utiliser Token2Pay! 💳\n\n" +
                     $"Date: {DateTime.Now:dd/MM/yyyy HH:mm}";

        return await SendMessageAsync(phoneNumber, message);
    }

    public async Task<bool> SendMessageAsync(string phoneNumber, string message)
    {
        try
        {
            // Check if Twilio is configured
            if (string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken) || string.IsNullOrEmpty(_whatsAppFromNumber))
            {
                _logger.LogWarning("Twilio is not configured. WhatsApp message not sent. Please configure Twilio settings in appsettings.json");
                return false;
            }

            // Format phone number (remove spaces, add country code if needed)
            var formattedPhone = FormatPhoneNumber(phoneNumber);

            // Send WhatsApp message via Twilio
            // Note: For business-initiated messages (first message), Twilio requires a template
            // Once the user replies, you can send free-form messages for 24 hours
            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber($"whatsapp:{_whatsAppFromNumber}"),
                to: new PhoneNumber($"whatsapp:{formattedPhone}")
            );

            _logger.LogInformation("WhatsApp message sent successfully. SID: {MessageSid}", messageResource.Sid);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending WhatsApp message to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    private string FormatPhoneNumber(string phoneNumber)
    {
        // Remove all non-digit characters except +
        var cleaned = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        // If it doesn't start with +, assume it needs country code
        if (!cleaned.StartsWith("+"))
        {
            // Default to Morocco (+212) if no country code
            if (cleaned.StartsWith("0"))
            {
                cleaned = "+212" + cleaned.Substring(1);
            }
            else if (!cleaned.StartsWith("212"))
            {
                cleaned = "+212" + cleaned;
            }
            else
            {
                cleaned = "+" + cleaned;
            }
        }

        return cleaned;
    }

    private string GetCurrencySymbol()
    {
        return _configuration["Currency:Symbol"] ?? "USD";
    }
}
