using HotelStay.Application.Contracts;
using HotelStay.Application.Services;
using HotelStay.Domain.Enums;

namespace HotelStay.UnitTests;

public sealed class HotelDocumentValidationServiceTests
{
    [Fact]
    public async Task ValidateDocumentAsync_ReturnsValidForDomesticDestinationWithNationalId()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Delhi", DocumentType.NationalId, "NID-123");

        Assert.True(result.IsValid);
        Assert.Equal("Document accepted.", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_ReturnsInvalidForDomesticDestinationWithPassport()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Delhi", DocumentType.Passport, "PASSPORT-123");

        Assert.False(result.IsValid);
        Assert.Equal("Domestic destinations require a NationalId.", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_ReturnsValidForInternationalDestinationWithPassport()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Paris", DocumentType.Passport, "PASSPORT-456");

        Assert.True(result.IsValid);
        Assert.Equal("Document accepted.", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_ReturnsInvalidForInternationalDestinationWithNationalId()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Paris", DocumentType.NationalId, "NID-456");

        Assert.False(result.IsValid);
        Assert.Equal("International destinations require a Passport.", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_ReturnsInvalidWhenDocumentNumberIsMissing()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Delhi", DocumentType.NationalId, string.Empty);

        Assert.False(result.IsValid);
        Assert.Equal("Document number is required.", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_ReturnsInvalidWhenDocumentNumberIsWhitespace()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Delhi", DocumentType.NationalId, "   ");

        Assert.False(result.IsValid);
        Assert.Equal("Document number is required.", result.Message);
    }

    private sealed class FakeDestinationCategorySource : IDestinationCategorySource
    {
        public Task<IReadOnlyCollection<string>> GetDomesticCitiesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<string>>(new[] { "delhi", "mumbai" });
        }

        public Task<IReadOnlyCollection<string>> GetInternationalCitiesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<string>>(new[] { "paris", "london" });
        }
    }
}
