using System.Net;
using System.Net.Http.Json;
using HotelStay.Api.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelStay.UnitTests;

public sealed class ApiIntegrationTests
{
    [Fact]
    public async Task SearchEndpoint_ReturnsOffersForValidCriteria()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/hotels/search?destination=Delhi&checkIn=2026-07-11&checkOut=2026-07-14");

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<SearchHotelsResponse>();

        Assert.NotNull(payload);
        Assert.Equal(3, payload!.Results.Count);
        Assert.Collection(payload.Results,
            first => Assert.Equal("premier-1", first.Id),
            second => Assert.Equal("premier-2", second.Id),
            third => Assert.Equal("budget-2", third.Id));
    }

    [Fact]
    public async Task SearchEndpoint_ReturnsBadRequestForInvalidDates()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/hotels/search?destination=Delhi&checkIn=2026-07-14&checkOut=2026-07-11");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("VALIDATION_ERROR", error!.Code);
    }

    [Fact]
    public async Task ReserveEndpoint_CreatesReservationAndAllowsLookup()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var searchResponse = await client.GetAsync("/hotels/search?destination=Delhi&checkIn=2026-07-11&checkOut=2026-07-14");
        searchResponse.EnsureSuccessStatusCode();

        var reservationRequest = new ReserveHotelRequest(
            TravellerName: "Alex",
            Destination: "Delhi",
            DocumentType: "NationalId",
            DocumentNumber: "NID-123",
            SelectedOfferId: "premier-1");

        var reserveResponse = await client.PostAsJsonAsync("/hotels/reserve", reservationRequest);
        reserveResponse.EnsureSuccessStatusCode();

        var reservationPayload = await reserveResponse.Content.ReadFromJsonAsync<ReservationDto>();
        Assert.NotNull(reservationPayload);
        Assert.Equal("PremierStays", reservationPayload!.Provider);
        Assert.Equal("premier-1", reservationPayload.OfferSnapshot!.Id);

        var lookupResponse = await client.GetAsync($"/hotels/reservation/{reservationPayload.ReservationReference}");
        lookupResponse.EnsureSuccessStatusCode();

        var lookupPayload = await lookupResponse.Content.ReadFromJsonAsync<ReservationLookupDto>();
        Assert.NotNull(lookupPayload);
        Assert.Equal(reservationPayload.ReservationReference, lookupPayload!.ReservationReference);
    }

    [Fact]
    public async Task ReserveEndpoint_ReturnsUnprocessableEntityForBusinessRuleViolation()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var request = new ReserveHotelRequest(
            TravellerName: "Alex",
            Destination: "London",
            DocumentType: "NationalId",
            DocumentNumber: "NID-123",
            SelectedOfferId: "premier-1");

        var response = await client.PostAsJsonAsync("/hotels/reserve", request);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("BUSINESS_RULE_VIOLATION", error!.Code);
        Assert.Contains("International destinations require a Passport", error.Message);
    }

    [Fact]
    public async Task ReservationLookupEndpoint_ReturnsNotFoundForUnknownReference()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/hotels/reservation/UNKNOWN-REF");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("NOT_FOUND", error!.Code);
    }

    [Fact]
    public async Task ReserveEndpoint_ReturnsBadRequest_WhenOfferNotStored()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var request = new ReserveHotelRequest(
            TravellerName: "Alex",
            Destination: "Delhi",
            DocumentType: "NationalId",
            DocumentNumber: "NID-123",
            SelectedOfferId: "premier-1",
            OfferSnapshot: null);

        var response = await client.PostAsJsonAsync("/hotels/reserve", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("INVALID_REQUEST", error!.Code);
    }

    [Fact]
    public async Task ReserveEndpoint_AllowsColdStartReservationWithOfferSnapshot()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var snapshot = new HotelOfferDto(
            "premier-1",
            "PremierStays",
            "Standard",
            120m,
            360m,
            "FreeCancellation",
            48);

        var request = new ReserveHotelRequest(
            TravellerName: "Alex",
            Destination: "Delhi",
            DocumentType: "NationalId",
            DocumentNumber: "NID-123",
            SelectedOfferId: null,
            OfferSnapshot: snapshot);

        var response = await client.PostAsJsonAsync("/hotels/reserve", request);
        response.EnsureSuccessStatusCode();

        var reservationPayload = await response.Content.ReadFromJsonAsync<ReservationDto>();
        Assert.NotNull(reservationPayload);
        Assert.Equal("PremierStays", reservationPayload!.Provider);
        Assert.Equal("premier-1", reservationPayload.OfferSnapshot!.Id);
        Assert.Equal(360m, reservationPayload.TotalPrice);
    }
}
