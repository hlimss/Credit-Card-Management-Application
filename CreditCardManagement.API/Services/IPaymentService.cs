using CreditCardManagement.API.DTOs;

namespace CreditCardManagement.API.Services;

public interface IPaymentService
{
    Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createDto, Guid userId);
    Task<IEnumerable<PaymentDto>> GetUserPaymentsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId, Guid userId);
    Task<IEnumerable<string>> GetAvailablePaymentTypesAsync();
}
