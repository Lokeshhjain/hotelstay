using HotelStay.Application.Contracts;
using HotelStay.Domain.Enums;
using HotelStay.Domain.Services;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Application.Services;

public sealed class HotelDocumentValidationService : IHotelDocumentValidationService
{
    private readonly IDestinationCategorySource _destinationCategorySource;

    public HotelDocumentValidationService(IDestinationCategorySource destinationCategorySource)
    {
        _destinationCategorySource = destinationCategorySource;
    }

    public async Task<DocumentValidationResult> ValidateDocumentAsync(string destination, DocumentType documentType, string documentNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var domesticCities = await _destinationCategorySource.GetDomesticCitiesAsync(cancellationToken);
            var internationalCities = await _destinationCategorySource.GetInternationalCitiesAsync(cancellationToken);

            var category = HotelBusinessRules.DetermineDestinationCategory(destination, domesticCities, internationalCities);
            var requiredDocument = category == DestinationCategory.International ? DocumentType.Passport : DocumentType.NationalId;

            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                return new DocumentValidationResult(false, "Document number is required.");
            }

            if (documentType != requiredDocument)
            {
                return new DocumentValidationResult(false, $"{category} destinations require a {requiredDocument}.");
            }

            return new DocumentValidationResult(true, "Document accepted.");
        }
        catch (ArgumentException ex)
        {
            // Destination is not recognized
            return new DocumentValidationResult(false, ex.Message);
        }
    }
}
