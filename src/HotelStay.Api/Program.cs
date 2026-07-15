using HotelStay.Api.Routes;
using HotelStay.Application.Contracts;
using HotelStay.Application.Services;
using HotelStay.Infrastructure;
using HotelStay.Infrastructure.Mappers;
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
builder.Services.AddSingleton<IProviderOfferMapper<HotelStay.Infrastructure.Mappers.PremierStaysOfferResponse>, PremierStaysMapper>();
builder.Services.AddSingleton<IProviderOfferMapper<HotelStay.Infrastructure.Mappers.BudgetNestsOfferResponse>, BudgetNestsMapper>();
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

app.MapHotelRoutes();

app.Run();

public partial class Program { }
