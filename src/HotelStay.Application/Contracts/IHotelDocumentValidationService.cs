using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Application.Contracts;

public interface IHotelDocumentValidationService
{
    Task<DocumentValidationResult> ValidateDocumentAsync(string destination, DocumentType documentType, string documentNumber, CancellationToken cancellationToken = default);
}
