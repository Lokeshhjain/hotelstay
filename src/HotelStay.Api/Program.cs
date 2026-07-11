using HotelStay.Application.Contracts;
using HotelStay.Application.Services;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;
using HotelStay.Infrastructure;
using HotelStay.Infrastructure.Providers;
using HotelStay.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<InMemoryDataContext>();
builder.Services.AddSingleton<IDestinationCategorySource, DestinationCategorySource>();
builder.Services.AddSingleton<IReservationStore, InMemoryReservationStore>();
builder.Services.AddSingleton<IHotelProvider, PremierStaysProvider>();
builder.Services.AddSingleton<IHotelProvider, BudgetNestsProvider>();
builder.Services.AddScoped<HotelDocumentValidationService>();
builder.Services.AddScoped<HotelAvailabilityService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // only if using cookies/auth
    });
});

var app = builder.Build();

app.UseCors("AngularPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/hotels/search", async (string destination, DateOnly checkIn, DateOnly checkOut, string? roomType, HotelAvailabilityService service, CancellationToken cancellationToken) =>
{
    try
    {
        var criteria = new SearchCriteria(destination, checkIn, checkOut, roomType);
        var offers = await service.SearchHotelsAsync(criteria, cancellationToken);
        return Results.Ok(new
        {
            results = offers.Select(offer => new HotelOfferResponse(
                offer.Id,
                offer.Provider,
                offer.RoomType.ToString(),
                offer.PerNightRate,
                offer.TotalStayPrice,
                offer.CancellationPolicy.ToString()))
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/hotels/reserve", async (ReserveHotelRequest request, HotelAvailabilityService service, CancellationToken cancellationToken) =>
{
    if (!Enum.TryParse<DocumentType>(request.DocumentType, true, out var documentType))
    {
        return Results.BadRequest(new { error = "Document type is invalid." });
    }

    try
    {
        var reservationRequest = new ReservationRequest(
            request.TravellerName,
            request.Destination,
            documentType,
            request.DocumentNumber,
            request.SelectedOfferId);

        var reservation = await service.ReserveHotelAsync(reservationRequest, cancellationToken);
        return Results.Ok(new ReservationResponse(
            reservation.ReservationReference.Value,
            reservation.Provider,
            reservation.TotalPrice,
            reservation.CancellationPolicy.ToString()));
    }
    catch (InvalidOperationException ex)
    {
        return Results.UnprocessableEntity(new { error = ex.Message });
    }
});

app.MapGet("/hotels/reservation/{reference}", async (string reference, HotelAvailabilityService service, CancellationToken cancellationToken) =>
{
    var reservation = await service.GetReservationAsync(reference, cancellationToken);

    return reservation is null
        ? Results.NotFound(new { error = "Reservation was not found." })
        : Results.Ok(new ReservationLookupResponse(
            reservation.ReservationReference.Value,
            reservation.TravellerName,
            reservation.Provider,
            reservation.RoomType.ToString(),
            reservation.TotalPrice,
            reservation.CancellationPolicy.ToString(),
            reservation.ValidationOutcome));
});

app.Run();

internal sealed record HotelOfferResponse(string Id, string Provider, string RoomType, decimal PerNightRate, decimal TotalStayPrice, string CancellationPolicy);

internal sealed record ReservationResponse(string ReservationReference, string Provider, decimal TotalPrice, string CancellationPolicy);

internal sealed record ReservationLookupResponse(string ReservationReference, string TravellerName, string Provider, string RoomType, decimal TotalPrice, string CancellationPolicy, string ValidationOutcome);

internal sealed record ReserveHotelRequest(string TravellerName, string Destination, string DocumentType, string DocumentNumber, string SelectedOfferId);
