using HotelStay.Api.Contracts;
using HotelStay.Api.Validation;
using HotelStay.Application.Contracts;
using HotelStay.Application.Services;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;
using HotelStay.Infrastructure;
using HotelStay.Infrastructure.Providers;
using HotelStay.Infrastructure.Repositories;
using HotelStay.Infrastructure.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<InMemoryDataContext>();
builder.Services.AddSingleton<IDestinationCategorySource, DestinationCategorySource>();
builder.Services.AddSingleton<IReservationStore, InMemoryReservationStore>();
builder.Services.AddSingleton<IOfferCatalog, InMemoryOfferCatalog>();
builder.Services.AddSingleton<IHotelProvider, PremierStaysProvider>();
builder.Services.AddSingleton<IHotelProvider, BudgetNestsProvider>();
builder.Services.AddScoped<IHotelDocumentValidationService, HotelDocumentValidationService>();
builder.Services.AddScoped<IHotelAvailabilityService, HotelAvailabilityService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AngularPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/hotels/search", async (string? destination, DateOnly? checkIn, DateOnly? checkOut, string? roomType, IHotelAvailabilityService service, CancellationToken cancellationToken) =>
{
    var request = new SearchHotelsRequest(destination, checkIn, checkOut, roomType);
    var errors = HotelRequestValidator.ValidateSearch(request);

    if (errors.Count > 0)
    {
        return Results.BadRequest(new ApiErrorResponse("VALIDATION_ERROR", "Search request validation failed.", errors));
    }

    var criteria = new SearchCriteria(destination!, checkIn!.Value, checkOut!.Value, roomType);
    var offers = await service.SearchHotelsAsync(criteria, cancellationToken);

    return Results.Ok(new SearchHotelsResponse(
        offers.Select(offer => new HotelOfferDto(
            offer.Id,
            offer.Provider,
            offer.RoomType.ToString(),
            offer.PerNightRate,
            offer.TotalStayPrice,
            offer.CancellationPolicy.ToString())).ToList()));
});

app.MapPost("/hotels/reserve", async (ReserveHotelRequest request, IHotelAvailabilityService service, CancellationToken cancellationToken) =>
{
    var errors = HotelRequestValidator.ValidateReservation(request);

    if (errors.Count > 0)
    {
        var code = errors.Any(x => x.Contains("invalid", StringComparison.OrdinalIgnoreCase)) ? "VALIDATION_ERROR" : "INVALID_REQUEST";
        return Results.BadRequest(new ApiErrorResponse(code, "Reservation request validation failed.", errors));
    }

    if (!Enum.TryParse<DocumentType>(request.DocumentType!, true, out var documentType))
    {
        return Results.BadRequest(new ApiErrorResponse("INVALID_REQUEST", "Reservation request validation failed.", new[] { "Document type is invalid." }));
    }

    try
    {
        var reservationRequest = new HotelStay.Domain.ValueObjects.ReservationRequest(
            request.TravellerName!,
            request.Destination!,
            documentType,
            request.DocumentNumber!,
            request.SelectedOfferId!);

        var reservation = await service.ReserveHotelAsync(reservationRequest, cancellationToken);

        return Results.Ok(new ReservationDto(
            reservation.ReservationReference.Value,
            reservation.Provider,
            reservation.TotalPrice,
            reservation.CancellationPolicy.ToString()));
    }
    catch (InvalidOperationException ex)
    {
        return Results.UnprocessableEntity(new ApiErrorResponse("BUSINESS_RULE_VIOLATION", ex.Message, new[] { ex.Message }));
    }
});

app.MapGet("/hotels/reservation/{reference}", async (string reference, IHotelAvailabilityService service, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(reference))
    {
        return Results.BadRequest(new ApiErrorResponse("INVALID_REQUEST", "Reservation reference is required.", new[] { "Reservation reference is required." }));
    }

    var reservation = await service.GetReservationAsync(reference, cancellationToken);

    return reservation is null
        ? Results.NotFound(new ApiErrorResponse("NOT_FOUND", "Reservation was not found.", new[] { "Reservation was not found." }))
        : Results.Ok(new ReservationLookupDto(
            reservation.ReservationReference.Value,
            reservation.TravellerName,
            reservation.Provider,
            reservation.RoomType.ToString(),
            reservation.TotalPrice,
            reservation.CancellationPolicy.ToString(),
            reservation.ValidationOutcome));
});

app.Run();
