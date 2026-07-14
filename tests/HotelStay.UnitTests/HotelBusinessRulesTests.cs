using HotelStay.Domain.Enums;
using HotelStay.Domain.Services;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.UnitTests;

public sealed class HotelBusinessRulesTests
{
    [Fact]
    public void ValidateSearchCriteria_ThrowsWhenDestinationMissing()
    {
        var criteria = new SearchCriteria(string.Empty, new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 12), null);

        var exception = Assert.Throws<ArgumentException>(() => HotelBusinessRules.ValidateSearchCriteria(criteria));

        Assert.Contains("Destination is required.", exception.Message);
    }

    [Fact]
    public void ValidateSearchCriteria_ThrowsWhenCheckOutIsNotAfterCheckIn()
    {
        var criteria = new SearchCriteria("Delhi", new DateOnly(2026, 7, 12), new DateOnly(2026, 7, 12), null);

        var exception = Assert.Throws<ArgumentException>(() => HotelBusinessRules.ValidateSearchCriteria(criteria));

        Assert.Contains("Check-out date must be after check-in date.", exception.Message);
    }

    [Theory]
    [InlineData("suite", RoomType.Suite)]
    [InlineData("deluxe", RoomType.Deluxe)]
    [InlineData("anything-else", RoomType.Standard)]
    [InlineData(null, RoomType.Standard)]
    public void MapRoomType_MapsToExpectedUnifiedRoomType(string? input, RoomType expected)
    {
        var mapped = HotelBusinessRules.MapRoomType(input);

        Assert.Equal(expected, mapped);
    }

    [Fact]
    public void DetermineDestinationCategory_ReturnsDomesticForKnownDomesticCity()
    {
        var domesticCities = new[] { "delhi", "mumbai" };
        var internationalCities = new[] { "london", "paris" };

        var domestic = HotelBusinessRules.DetermineDestinationCategory("Delhi", domesticCities, internationalCities);

        Assert.Equal(DestinationCategory.Domestic, domestic);
    }

    [Fact]
    public void DetermineDestinationCategory_ReturnsInternationalForKnownInternationalCity()
    {
        var domesticCities = new[] { "delhi", "mumbai" };
        var internationalCities = new[] { "london", "paris" };

        var international = HotelBusinessRules.DetermineDestinationCategory("Paris", domesticCities, internationalCities);

        Assert.Equal(DestinationCategory.International, international);
    }

    [Fact]
    public void DetermineDestinationCategory_ThrowsForUnknownDestination()
    {
        var domesticCities = new[] { "delhi", "mumbai" };
        var internationalCities = new[] { "london", "paris" };

        var exception = Assert.Throws<ArgumentException>(() => 
            HotelBusinessRules.DetermineDestinationCategory("Unknown City", domesticCities, internationalCities));

        Assert.Contains("not a recognized destination", exception.Message);
    }

    [Fact]
    public void DetermineDestinationCategory_IsCaseInsensitive()
    {
        var domesticCities = new[] { "delhi", "mumbai" };
        var internationalCities = new[] { "london", "paris" };

        var domestic = HotelBusinessRules.DetermineDestinationCategory("DELHI", domesticCities, internationalCities);
        var international = HotelBusinessRules.DetermineDestinationCategory("PARIS", domesticCities, internationalCities);

        Assert.Equal(DestinationCategory.Domestic, domestic);
        Assert.Equal(DestinationCategory.International, international);
    }
}
