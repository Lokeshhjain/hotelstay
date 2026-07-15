using System.Net;
using System.Net.Http.Json;
using HotelStay.Api.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelStay.UnitTests;

public sealed class HotelRoutesIntegrationTests
{
    [Fact]
    public async Task SearchReserveLookup_WorksEndToEndThroughHttpPipeline()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var searchResponse = await client.GetAsync("/hotels/search?destination=Delhi&checkIn=2026-07-11&checkOut=2026-07-14&roomType=suite");
        searchResponse.EnsureSuccessStatusCode();

        var searchPayload = await searchResponse.Content.ReadFromJsonAsync<SearchHotelsResponse>();
        Assert.NotNull(searchPayload);
        Assert.Single(searchPayload!.Results);
        Assert.Equal("budget-2", searchPayload.Results[0].Id);

        var reserveRequest = new ReserveHotelRequest(
            TravellerName: "Alex",
            Destination: "Delhi",
            DocumentType: "NationalId",
            DocumentNumber: "NID-123",
            SelectedOfferId: "budget-2",
            OfferSnapshot: searchPayload.Results[0]);

        var reserveResponse = await client.PostAsJsonAsync("/hotels/reserve", reserveRequest);
        reserveResponse.EnsureSuccessStatusCode();

        var reservePayload = await reserveResponse.Content.ReadFromJsonAsync<ReservationDto>();
        Assert.NotNull(reservePayload);
        Assert.Equal("BudgetNests", reservePayload!.Provider);
        Assert.Equal("budget-2", reservePayload.OfferSnapshot!.Id);

        var lookupResponse = await client.GetAsync($"/hotels/reservation/{reservePayload.ReservationReference}");
        lookupResponse.EnsureSuccessStatusCode();

        var lookupPayload = await lookupResponse.Content.ReadFromJsonAsync<ReservationLookupDto>();
        Assert.NotNull(lookupPayload);
        Assert.Equal(reservePayload.ReservationReference, lookupPayload!.ReservationReference);
        Assert.Equal("BudgetNests", lookupPayload.Provider);
    }

    [Fact]
    public async Task Search_ReturnsBadRequestForInvalidRoomType()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/hotels/search?destination=Delhi&checkIn=2026-07-11&checkOut=2026-07-14&roomType=penthouse");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("VALIDATION_ERROR", error!.Code);
        Assert.Contains("Room type must be Standard, Deluxe, or Suite.", error.Details!);
    }
}
