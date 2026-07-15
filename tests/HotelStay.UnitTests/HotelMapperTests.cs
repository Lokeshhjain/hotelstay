using System.Text.Json;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;
using HotelStay.Infrastructure.Mappers;

namespace HotelStay.UnitTests;

public sealed class HotelMapperTests
{
    [Fact]
    public void PremierStaysRawResponse_SerializesUsingPascalCaseShape()
    {
        var json = JsonSerializer.Serialize(new PremierStaysOfferResponse(
            "premier-1",
            "PremierStays",
            "STANDARD",
            120m,
            "FreeCancellation",
            true,
            48,
            "Delhi"));

        Assert.Contains("\"Id\"", json);
        Assert.Contains("\"ProviderName\"", json);
        Assert.Contains("\"RoomTypeCode\"", json);
    }

    [Fact]
    public void BudgetNestsRawResponse_SerializesUsingSnakeCaseShape()
    {
        var json = JsonSerializer.Serialize(new BudgetNestsOfferResponse(
            "budget-2",
            "BudgetNests",
            "suite",
            220m,
            "Flexible",
            true,
            24,
            "Delhi"));

        Assert.Contains("\"offer_id\"", json);
        Assert.Contains("\"provider_name\"", json);
        Assert.Contains("\"room_type_code\"", json);
    }

    [Fact]
    public void PremierStaysMapper_NormalizesPascalCaseSource()
    {
        var mapper = new PremierStaysMapper();
        var source = new PremierStaysOfferResponse(
            "premier-1",
            "PremierStays",
            "STANDARD",
            120m,
            "FreeCancellation",
            true,
            48,
            "Delhi");

        var mapped = mapper.Map(source, new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        Assert.Equal("premier-1", mapped.Id);
        Assert.Equal("PremierStays", mapped.ProviderName);
        Assert.Equal("STANDARD", mapped.RoomTypeCode);
        Assert.Equal(CancellationPolicy.FreeCancellation, mapped.CancellationPolicy);
        Assert.Equal(48, mapped.CancellationWindowHoursBeforeCheckIn);
    }

    [Fact]
    public void BudgetNestsMapper_NormalizesSnakeCaseSource()
    {
        var mapper = new BudgetNestsMapper();
        var source = new BudgetNestsOfferResponse(
            "budget-2",
            "BudgetNests",
            "suite",
            220m,
            "Flexible",
            true,
            24,
            "Delhi");

        var mapped = mapper.Map(source, new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        Assert.Equal("budget-2", mapped.Id);
        Assert.Equal("BudgetNests", mapped.ProviderName);
        Assert.Equal("suite", mapped.RoomTypeCode);
        Assert.Equal(CancellationPolicy.Flexible, mapped.CancellationPolicy);
        Assert.Equal(24, mapped.CancellationWindowHoursBeforeCheckIn);
    }
}
