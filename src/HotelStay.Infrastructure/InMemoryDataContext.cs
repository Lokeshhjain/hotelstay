using HotelStay.Application.Models;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Infrastructure;

public sealed class InMemoryDataContext
{
    public InMemoryDataContext()
    {
        PremierStaysOffers = new List<ProviderHotelOffer>
        {
            new("premier-1", "PremierStays", "standard", 120m, CancellationPolicy.FreeCancellation, true, 48),
            new("premier-2", "PremierStays", "deluxe", 180m, CancellationPolicy.NonRefundable, true, 0)
        };

        BudgetNestsOffers = new List<ProviderHotelOffer>
        {
            new("budget-1", "BudgetNests", "standard", 90m, CancellationPolicy.NonRefundable, false, 0),
            new("budget-2", "BudgetNests", "suite", 220m, CancellationPolicy.Flexible, true, 24)
        };

        DomesticCities = new List<string>
        {
            "india", "delhi", "mumbai", "bengaluru", "bangalore", "chennai", "hyderabad"
        };

        InternationalCities = new List<string>
        {
            "london", "paris", "tokyo", "new york", "toronto", "sydney"
        };
    }

    public IReadOnlyCollection<ProviderHotelOffer> PremierStaysOffers { get; }

    public IReadOnlyCollection<ProviderHotelOffer> BudgetNestsOffers { get; }

    public IReadOnlyCollection<string> DomesticCities { get; }

    public IReadOnlyCollection<string> InternationalCities { get; }
}
