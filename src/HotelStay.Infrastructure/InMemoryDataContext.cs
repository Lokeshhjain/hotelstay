using HotelStay.Application.Models;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Infrastructure;

public sealed class InMemoryDataContext
{
    public InMemoryDataContext()
    {
        // Destination-scoped stub offers. Each offer includes the destination it belongs to.
        PremierStaysOffers = new List<ProviderHotelOffer>
        {
            new("premier-1", "PremierStays", "standard", 120m, CancellationPolicy.FreeCancellation, true, 48, "Delhi"),
            new("premier-2", "PremierStays", "deluxe", 180m, CancellationPolicy.NonRefundable, true, 0, "Delhi"),
            new("premier-3", "PremierStays", "standard", 130m, CancellationPolicy.Flexible, true, 24, "Mumbai"),
            new("premier-bengaluru-1", "PremierStays", "suite", 240m, CancellationPolicy.Flexible, true, 24, "Bengaluru"),
            new("premier-chennai-1", "PremierStays", "standard", 110m, CancellationPolicy.FreeCancellation, true, 48, "Chennai"),
            new("premier-london-1", "PremierStays", "standard", 150m, CancellationPolicy.FreeCancellation, true, 48, "London"),
            new("premier-paris-1", "PremierStays", "deluxe", 230m, CancellationPolicy.Flexible, true, 24, "Paris"),
            new("premier-tokyo-1", "PremierStays", "suite", 260m, CancellationPolicy.NonRefundable, true, 0, "Tokyo")
        };

        BudgetNestsOffers = new List<ProviderHotelOffer>
        {
            new("budget-1", "BudgetNests", "standard", 90m, CancellationPolicy.NonRefundable, false, 0, "Delhi"),
            new("budget-2", "BudgetNests", "suite", 220m, CancellationPolicy.Flexible, true, 24, "Delhi"),
            new("budget-3", "BudgetNests", "standard", 85m, CancellationPolicy.NonRefundable, true, 0, "Mumbai"),
            new("budget-bengaluru-1", "BudgetNests", "deluxe", 170m, CancellationPolicy.Flexible, true, 24, "Bengaluru"),
            new("budget-chennai-1", "BudgetNests", "standard", 95m, CancellationPolicy.FreeCancellation, true, 24, "Chennai"),
            new("budget-london-1", "BudgetNests", "deluxe", 210m, CancellationPolicy.Flexible, true, 24, "London"),
            new("budget-paris-1", "BudgetNests", "standard", 140m, CancellationPolicy.FreeCancellation, true, 48, "Paris"),
            new("budget-tokyo-1", "BudgetNests", "suite", 250m, CancellationPolicy.NonRefundable, true, 0, "Tokyo")
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
