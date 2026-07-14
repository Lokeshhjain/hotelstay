using HotelStay.Application.Contracts;
using HotelStay.Application.Services;
using HotelStay.Domain.Enums;

namespace HotelStay.UnitTests;

/// <summary>
/// Edge case and validation boundary tests for HotelDocumentValidationService.
/// Covers whitespace handling, special characters, unknown destinations, null safety,
/// and edge cases in document number validation.
/// </summary>
public sealed class DocumentValidationEdgeCasesTests
{
    [Theory]
    [InlineData("NID-123")]
    [InlineData("NID123")]
    [InlineData("123")]
    [InlineData("ABC123DEF")]
    [InlineData("X")]
    public async Task ValidateDocumentAsync_AcceptsValidDocumentNumbers(string docNumber)
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Delhi", DocumentType.NationalId, docNumber);

        Assert.True(result.IsValid);
        Assert.Equal("Document accepted.", result.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("\t")]
    [InlineData("\r\n")]
    public async Task ValidateDocumentAsync_RejectsEmptyOrWhitespaceDocumentNumber(string docNumber)
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Delhi", DocumentType.NationalId, docNumber);

        Assert.False(result.IsValid);
        Assert.Equal("Document number is required.", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_RejectsUnknownDestination()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("UnknownCity", DocumentType.NationalId, "NID-123");

        Assert.False(result.IsValid);
        Assert.Contains("not a recognized destination", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_HandlesCaseInsensitiveDestination()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("DELHI", DocumentType.NationalId, "NID-123");

        Assert.True(result.IsValid);
        Assert.Equal("Document accepted.", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_HandlesDestinationWithWhitespace()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("  Delhi  ", DocumentType.NationalId, "NID-123");

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateDocumentAsync_RejectsDomesticWithPassport()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Delhi", DocumentType.Passport, "PASSPORT-456");

        Assert.False(result.IsValid);
        Assert.Equal("Domestic destinations require a NationalId.", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_RejectsInternationalWithNationalId()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Paris", DocumentType.NationalId, "NID-456");

        Assert.False(result.IsValid);
        Assert.Equal("International destinations require a Passport.", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_AcceptsDomesticWithNationalId()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Mumbai", DocumentType.NationalId, "NID-789");

        Assert.True(result.IsValid);
        Assert.Equal("Document accepted.", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_AcceptsInternationalWithPassport()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("London", DocumentType.Passport, "PASSPORT-789");

        Assert.True(result.IsValid);
        Assert.Equal("Document accepted.", result.Message);
    }

    [Theory]
    [InlineData("Delhi")]
    [InlineData("delhi")]
    [InlineData("DELHI")]
    [InlineData("DeLhI")]
    [InlineData("  Delhi  ")]
    public async Task ValidateDocumentAsync_AllVariantsOfValidDomesticCitiesWork(string destination)
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync(destination, DocumentType.NationalId, "NID-123");

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("Paris")]
    [InlineData("paris")]
    [InlineData("PARIS")]
    [InlineData("PaRiS")]
    [InlineData("  London  ")]
    public async Task ValidateDocumentAsync_AllVariantsOfValidInternationalCitiesWork(string destination)
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync(destination, DocumentType.Passport, "PASSPORT-123");

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("Del")]
    [InlineData("Pari")]
    [InlineData("Delh")]
    [InlineData("Londnn")]
    public async Task ValidateDocumentAsync_RejectsPartialCityMatches(string partialCity)
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync(partialCity, DocumentType.NationalId, "NID-123");

        Assert.False(result.IsValid);
        Assert.Contains("not a recognized destination", result.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_RejectsEmptyDestination()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync(string.Empty, DocumentType.NationalId, "NID-123");

        Assert.False(result.IsValid);
        Assert.Contains("not a recognized destination", result.Message);
    }

    [Theory]
    [InlineData("NID-WITH-SPECIAL-CHARS!@#")]
    [InlineData("NID_WITH_UNDERSCORES")]
    [InlineData("NID.WITH.DOTS")]
    [InlineData("NID/WITH/SLASHES")]
    [InlineData("NIDWITHVERYLONGNUMBER1234567890ABCDEFGHIJ")]
    public async Task ValidateDocumentAsync_AcceptsDocumentNumbersWithSpecialCharacters(string docNumber)
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Delhi", DocumentType.NationalId, docNumber);

        // Should accept as long as it's not empty/whitespace
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateDocumentAsync_HandlesMultipleSpacesInDocumentNumber()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result = await service.ValidateDocumentAsync("Delhi", DocumentType.NationalId, "NID    123");

        // Should accept if not all whitespace
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(DocumentType.NationalId)]
    [InlineData(DocumentType.Passport)]
    public async Task ValidateDocumentAsync_ValidateAllDocumentTypes(DocumentType docType)
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());
        var destination = docType == DocumentType.NationalId ? "Delhi" : "Paris";

        var result = await service.ValidateDocumentAsync(destination, docType, "TEST-123");

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateDocumentAsync_ConsistentErrorMessagesAcrossMultipleCalls()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result1 = await service.ValidateDocumentAsync("Delhi", DocumentType.Passport, "PASSPORT-123");
        var result2 = await service.ValidateDocumentAsync("Delhi", DocumentType.Passport, "OTHER-123");

        // Both should have the same error message
        Assert.False(result1.IsValid);
        Assert.False(result2.IsValid);
        Assert.Equal(result1.Message, result2.Message);
        Assert.Equal("Domestic destinations require a NationalId.", result1.Message);
    }

    [Fact]
    public async Task ValidateDocumentAsync_IsIdempotent()
    {
        var service = new HotelDocumentValidationService(new FakeDestinationCategorySource());

        var result1 = await service.ValidateDocumentAsync("Delhi", DocumentType.NationalId, "NID-123");
        var result2 = await service.ValidateDocumentAsync("Delhi", DocumentType.NationalId, "NID-123");

        Assert.Equal(result1.IsValid, result2.IsValid);
        Assert.Equal(result1.Message, result2.Message);
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
