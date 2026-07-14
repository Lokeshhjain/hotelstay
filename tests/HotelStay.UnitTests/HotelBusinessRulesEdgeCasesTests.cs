using HotelStay.Domain.Enums;
using HotelStay.Domain.Services;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.UnitTests;

/// <summary>
/// Edge case and boundary condition tests for HotelBusinessRules validation logic.
/// Covers whitespace handling, special characters, null safety, and boundary conditions.
/// </summary>
public sealed class HotelBusinessRulesEdgeCasesTests
{
    [Theory]
    [InlineData("  Delhi  ")]
    [InlineData("\tDelhi\t")]
    [InlineData("\r\nDelhi\r\n")]
    public void ValidateSearchCriteria_HandlesDestinationWithWhitespace(string destinationWithWhitespace)
    {
        var criteria = new SearchCriteria(destinationWithWhitespace, new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 12), null);

        // Should trim and not throw when whitespace is present
        var exception = Record.Exception(() => HotelBusinessRules.ValidateSearchCriteria(criteria));

        // Only empty after trim should cause failure
        if (destinationWithWhitespace.Trim() == string.Empty)
        {
            Assert.NotNull(exception);
            Assert.Contains("Destination is required.", exception!.Message);
        }
        else
        {
            Assert.Null(exception);
        }
    }

    [Fact]
    public void ValidateSearchCriteria_ThrowsWhenCheckInEqualsCheckOut()
    {
        var sameDate = new DateOnly(2026, 7, 15);
        var criteria = new SearchCriteria("Delhi", sameDate, sameDate, null);

        var exception = Assert.Throws<ArgumentException>(() => HotelBusinessRules.ValidateSearchCriteria(criteria));

        Assert.Contains("Check-out date must be after check-in date.", exception.Message);
    }

    [Fact]
    public void ValidateSearchCriteria_ThrowsWhenCheckOutIsBeforeCheckIn()
    {
        var criteria = new SearchCriteria("Delhi", new DateOnly(2026, 7, 20), new DateOnly(2026, 7, 15), null);

        var exception = Assert.Throws<ArgumentException>(() => HotelBusinessRules.ValidateSearchCriteria(criteria));

        Assert.Contains("Check-out date must be after check-in date.", exception.Message);
    }

    [Fact]
    public void ValidateSearchCriteria_AcceptsValidFutureDateRange()
    {
        var criteria = new SearchCriteria("Delhi", new DateOnly(2026, 12, 1), new DateOnly(2026, 12, 31), null);

        var exception = Record.Exception(() => HotelBusinessRules.ValidateSearchCriteria(criteria));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateSearchCriteria_AcceptsLongStayDuration()
    {
        // 90-day stay
        var criteria = new SearchCriteria("Delhi", new DateOnly(2026, 7, 1), new DateOnly(2026, 9, 29), null);

        var exception = Record.Exception(() => HotelBusinessRules.ValidateSearchCriteria(criteria));

        Assert.Null(exception);
    }

    [Fact]
    public void DetermineDestinationCategory_ThrowsWithDescriptiveErrorForUnknownCity()
    {
        var domesticCities = new[] { "delhi", "mumbai", "bangalore" };
        var internationalCities = new[] { "london", "paris", "tokyo" };
        var unknownCity = "RandomCity123";

        var exception = Assert.Throws<ArgumentException>(() =>
            HotelBusinessRules.DetermineDestinationCategory(unknownCity, domesticCities, internationalCities));

        Assert.Contains("not a recognized destination", exception.Message);
        Assert.Contains("RandomCity123", exception.Message);
        Assert.Contains("Valid destinations:", exception.Message);
    }

    [Theory]
    [InlineData("DELHI")]
    [InlineData("DeLhI")]
    [InlineData("delhi")]
    [InlineData("MUMBAI")]
    [InlineData("mumbai")]
    public void DetermineDestinationCategory_IsCaseInsensitiveForAllCities(string destination)
    {
        var domesticCities = new[] { "delhi", "mumbai" };
        var internationalCities = new[] { "london", "paris" };

        var category = HotelBusinessRules.DetermineDestinationCategory(destination, domesticCities, internationalCities);

        // Should be domestic for delhi/mumbai variants
        var isDomestic = destination.Equals("delhi", StringComparison.OrdinalIgnoreCase) ||
                         destination.Equals("mumbai", StringComparison.OrdinalIgnoreCase);

        if (isDomestic)
        {
            Assert.Equal(DestinationCategory.Domestic, category);
        }
        else
        {
            Assert.Equal(DestinationCategory.International, category);
        }
    }

    [Fact]
    public void DetermineDestinationCategory_TrimsWhitespaceFromDestination()
    {
        var domesticCities = new[] { "delhi", "mumbai" };
        var internationalCities = new[] { "london", "paris" };

        var category = HotelBusinessRules.DetermineDestinationCategory("  Delhi  ", domesticCities, internationalCities);

        Assert.Equal(DestinationCategory.Domestic, category);
    }

    [Fact]
    public void DetermineDestinationCategory_DoesNotMatchPartialCity()
    {
        var domesticCities = new[] { "delhi", "mumbai" };
        var internationalCities = new[] { "london", "paris" };

        // Should not match "Del" as substring of "Delhi"
        var exception = Assert.Throws<ArgumentException>(() =>
            HotelBusinessRules.DetermineDestinationCategory("Del", domesticCities, internationalCities));

        Assert.Contains("not a recognized destination", exception.Message);
    }

    [Fact]
    public void DetermineDestinationCategory_DoesNotMatchWithExtraWhitespace()
    {
        var domesticCities = new[] { "delhi" };
        var internationalCities = new[] { "london" };

        // "Delhi " with trailing space should still match after trim
        var category = HotelBusinessRules.DetermineDestinationCategory("Delhi ", domesticCities, internationalCities);

        Assert.Equal(DestinationCategory.Domestic, category);
    }

    [Theory]
    [InlineData("room")]
    [InlineData("SUITE")]
    [InlineData("standard")]
    [InlineData("DELUXE")]
    [InlineData("")]
    [InlineData(null)]
    public void MapRoomType_HandlesVariousCaseAndNullInputs(string? input)
    {
        var mapped = HotelBusinessRules.MapRoomType(input);

        if (string.IsNullOrEmpty(input) || input == "room")
        {
            Assert.Equal(RoomType.Standard, mapped);
        }
        else if (input.Equals("suite", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Equal(RoomType.Suite, mapped);
        }
        else if (input.Equals("deluxe", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Equal(RoomType.Deluxe, mapped);
        }
        else
        {
            Assert.Equal(RoomType.Standard, mapped);
        }
    }

    [Fact]
    public void ValidateSearchCriteria_AcceptsNullRoomType()
    {
        var criteria = new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 12), null);

        var exception = Record.Exception(() => HotelBusinessRules.ValidateSearchCriteria(criteria));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateSearchCriteria_AcceptsEmptyRoomType()
    {
        var criteria = new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 12), string.Empty);

        var exception = Record.Exception(() => HotelBusinessRules.ValidateSearchCriteria(criteria));

        Assert.Null(exception);
    }

    [Fact]
    public void DetermineDestinationCategory_WithEmptyCityCollections()
    {
        var domesticCities = Array.Empty<string>();
        var internationalCities = Array.Empty<string>();

        var exception = Assert.Throws<ArgumentException>(() =>
            HotelBusinessRules.DetermineDestinationCategory("Delhi", domesticCities, internationalCities));

        Assert.Contains("not a recognized destination", exception.Message);
    }

    [Fact]
    public void DetermineDestinationCategory_WithDuplicatesCitiesInCollections()
    {
        var domesticCities = new[] { "delhi", "delhi", "mumbai" };
        var internationalCities = new[] { "london", "london", "paris" };

        var domestic = HotelBusinessRules.DetermineDestinationCategory("Delhi", domesticCities, internationalCities);
        var international = HotelBusinessRules.DetermineDestinationCategory("London", domesticCities, internationalCities);

        Assert.Equal(DestinationCategory.Domestic, domestic);
        Assert.Equal(DestinationCategory.International, international);
    }

}
