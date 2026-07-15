using HotelStay.Domain.Enums;
using HotelStay.Infrastructure.Mappers;

namespace HotelStay.Infrastructure;

public sealed class InMemoryDataContext
{
    public InMemoryDataContext()
    {
        // Destination-scoped stub offers. Each offer includes the destination it belongs to.
        PremierStaysOffers = new List<PremierStaysOfferResponse>
        {
            new("premier-1", "PremierStays", "STANDARD", 120m, CancellationPolicy.FreeCancellation.ToString(), true, 48, "Delhi"),
            new("premier-2", "PremierStays", "DELUXE", 180m, CancellationPolicy.NonRefundable.ToString(), true, 0, "Delhi"),
            new("premier-3", "PremierStays", "STANDARD", 130m, CancellationPolicy.Flexible.ToString(), true, 24, "Mumbai"),
            new("premier-bengaluru-1", "PremierStays", "SUITE", 240m, CancellationPolicy.Flexible.ToString(), true, 24, "Bengaluru"),
            new("premier-chennai-1", "PremierStays", "STANDARD", 110m, CancellationPolicy.FreeCancellation.ToString(), true, 48, "Chennai"),
            new("premier-london-1", "PremierStays", "STANDARD", 150m, CancellationPolicy.FreeCancellation.ToString(), true, 48, "London"),
            new("premier-paris-1", "PremierStays", "DELUXE", 230m, CancellationPolicy.Flexible.ToString(), true, 24, "Paris"),
            new("premier-tokyo-1", "PremierStays", "SUITE", 260m, CancellationPolicy.NonRefundable.ToString(), true, 0, "Tokyo")
        };

        BudgetNestsOffers = new List<BudgetNestsOfferResponse>
        {
            new("budget-1", "BudgetNests", "standard", 90m, CancellationPolicy.NonRefundable.ToString(), false, 0, "Delhi"),
            new("budget-2", "BudgetNests", "suite", 220m, CancellationPolicy.Flexible.ToString(), true, 24, "Delhi"),
            new("budget-3", "BudgetNests", "standard", 85m, CancellationPolicy.NonRefundable.ToString(), true, 0, "Mumbai"),
            new("budget-bengaluru-1", "BudgetNests", "deluxe", 170m, CancellationPolicy.Flexible.ToString(), true, 24, "Bengaluru"),
            new("budget-chennai-1", "BudgetNests", "standard", 95m, CancellationPolicy.FreeCancellation.ToString(), true, 24, "Chennai"),
            new("budget-london-1", "BudgetNests", "deluxe", 210m, CancellationPolicy.Flexible.ToString(), true, 24, "London"),
            new("budget-paris-1", "BudgetNests", "standard", 140m, CancellationPolicy.FreeCancellation.ToString(), true, 48, "Paris"),
            new("budget-tokyo-1", "BudgetNests", "suite", 250m, CancellationPolicy.NonRefundable.ToString(), true, 0, "Tokyo")
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

    public IReadOnlyCollection<PremierStaysOfferResponse> PremierStaysOffers { get; }

    public IReadOnlyCollection<BudgetNestsOfferResponse> BudgetNestsOffers { get; }

    public IReadOnlyCollection<string> DomesticCities { get; }

    public IReadOnlyCollection<string> InternationalCities { get; }
}
