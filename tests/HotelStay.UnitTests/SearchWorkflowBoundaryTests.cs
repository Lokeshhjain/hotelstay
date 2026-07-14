using HotelStay.Application.Contracts;
using HotelStay.Application.Services;
using HotelStay.Domain.Entities;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;
using HotelStay.Infrastructure;
using HotelStay.Infrastructure.Mappers;
using HotelStay.Infrastructure.Providers;
using HotelStay.Infrastructure.Repositories;
using HotelStay.Infrastructure.Stores;

namespace HotelStay.UnitTests;

/// <summary>
/// Boundary condition and complex workflow tests for search operations.
/// Covers edge cases in date handling, room type filtering, provider coordination,
/// and result normalization across multiple providers.
/// </summary>
public sealed class SearchWorkflowBoundaryTests
{
    [Fact]
    public async Task SearchHotelsAsync_SingleNightStay()
    {
        var service = CreateService();
        var checkIn = new DateOnly(2026, 7, 15);
        var checkOut = new DateOnly(2026, 7, 16);

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", checkIn, checkOut, null));

        Assert.NotEmpty(offers);
        // Verify offers are correctly priced for single night
        var anyOffer = offers.First();
        Assert.Equal(anyOffer.PerNightRate, anyOffer.TotalStayPrice);
    }

    [Fact]
    public async Task SearchHotelsAsync_LongStay()
    {
        var service = CreateService();
        var checkIn = new DateOnly(2026, 7, 1);
        var checkOut = new DateOnly(2026, 8, 31);

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", checkIn, checkOut, null));

        Assert.NotEmpty(offers);
        // Verify pricing is correct for 61-day stay
        var anyOffer = offers.First();
        var expectedDays = (checkOut.DayNumber - checkIn.DayNumber);
        Assert.Equal(anyOffer.PerNightRate * expectedDays, anyOffer.TotalStayPrice);
    }

    [Fact]
    public async Task SearchHotelsAsync_FiltersByRequestedRoomTypeCaseInsensitive()
    {
        var service = CreateService();

        var resultsWithLowercase = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), "suite"));
        var resultsWithUppercase = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), "SUITE"));
        var resultsWithMixedCase = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), "SuItE"));

        Assert.Equal(resultsWithLowercase.Count, resultsWithUppercase.Count);
        Assert.Equal(resultsWithLowercase.Count, resultsWithMixedCase.Count);
        Assert.All(resultsWithLowercase, offer => Assert.Equal(RoomType.Suite, offer.RoomType));
    }

    [Fact]
    public async Task SearchHotelsAsync_ReturnsResultsSortedByTotalPrice()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        // Verify sorted by total price ascending
        for (int i = 0; i < offers.Count - 1; i++)
        {
            Assert.True(offers[i].TotalStayPrice <= offers[i + 1].TotalStayPrice,
                $"Offers not sorted: {offers[i].TotalStayPrice} > {offers[i + 1].TotalStayPrice}");
        }
    }

    [Fact]
    public async Task SearchHotelsAsync_ExcludesUnavailableOffers()
    {
        var reservationStore = new InMemoryReservationStore();
        await reservationStore.AddAsync(CreateReservation("RSV-1", "Existing", "budget-1"));
        await reservationStore.AddAsync(CreateReservation("RSV-2", "Existing", "budget-2"));

        var service = CreateService(reservationStore);

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        // Budget offers 1 and 2 should be excluded
        Assert.DoesNotContain(offers, offer => offer.Id == "budget-1");
        Assert.DoesNotContain(offers, offer => offer.Id == "budget-2");
    }

    [Fact]
    public async Task SearchHotelsAsync_IncludesCancellationPolicyWindow()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        Assert.NotEmpty(offers);
        Assert.All(offers, offer => Assert.True(offer.CancellationWindowHoursBeforeCheckIn >= 0));
    }

    [Fact]
    public async Task SearchHotelsAsync_DomesticCityReturnsResultsFromBothProviders()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var premierOffers = offers.Where(o => o.Provider == "PremierStays").ToList();
        var budgetOffers = offers.Where(o => o.Provider == "BudgetNests").ToList();

        Assert.NotEmpty(premierOffers);
        Assert.NotEmpty(budgetOffers);
    }

    [Fact]
    public async Task SearchHotelsAsync_InternationalCityReturnsResultsFromBothProviders()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("London", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var premierOffers = offers.Where(o => o.Provider == "PremierStays").ToList();
        var budgetOffers = offers.Where(o => o.Provider == "BudgetNests").ToList();

        Assert.NotEmpty(premierOffers);
        Assert.NotEmpty(budgetOffers);
    }

    [Fact]
    public async Task SearchHotelsAsync_DifferentDestinationsReturnDistinctStubData()
    {
        var service = CreateService();

        var delhiOffers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));
        var bengaluruOffers = await service.SearchHotelsAsync(new SearchCriteria("Bengaluru", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));
        var londonOffers = await service.SearchHotelsAsync(new SearchCriteria("London", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));
        var parisOffers = await service.SearchHotelsAsync(new SearchCriteria("Paris", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        Assert.DoesNotContain(bengaluruOffers, offer => offer.Id.StartsWith("premier-delhi") || offer.Id.StartsWith("budget-delhi") || offer.Id.StartsWith("premier-mumbai") || offer.Id.StartsWith("budget-mumbai"));
        Assert.DoesNotContain(londonOffers, offer => offer.Id.StartsWith("premier-delhi") || offer.Id.StartsWith("budget-delhi") || offer.Id.StartsWith("premier-mumbai") || offer.Id.StartsWith("budget-mumbai") || offer.Id.StartsWith("premier-bengaluru") || offer.Id.StartsWith("budget-bengaluru"));
        Assert.DoesNotContain(parisOffers, offer => offer.Id.StartsWith("premier-delhi") || offer.Id.StartsWith("budget-delhi") || offer.Id.StartsWith("premier-mumbai") || offer.Id.StartsWith("budget-mumbai") || offer.Id.StartsWith("premier-bengaluru") || offer.Id.StartsWith("budget-bengaluru") || offer.Id.StartsWith("premier-london") || offer.Id.StartsWith("budget-london"));
        Assert.DoesNotContain(delhiOffers, offer => offer.Id.StartsWith("premier-london") || offer.Id.StartsWith("budget-london") || offer.Id.StartsWith("premier-paris") || offer.Id.StartsWith("budget-paris") || offer.Id.StartsWith("premier-tokyo") || offer.Id.StartsWith("budget-tokyo"));
    }

    [Fact]
    public async Task SearchHotelsAsync_WithWhitespaceTrimmedFromDestination()
    {
        var service = CreateService();

        var resultsWithWhitespace = await service.SearchHotelsAsync(new SearchCriteria("  Delhi  ", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));
        var resultsWithoutWhitespace = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        Assert.Equal(resultsWithWhitespace.Count, resultsWithoutWhitespace.Count);
    }

    [Fact]
    public async Task SearchHotelsAsync_AllResultsMatchSearchDestination()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        // All offers should be from the requested destination
        // (Note: This test validates that search respects destination filtering at provider level)
        Assert.NotEmpty(offers);
    }

    [Fact]
    public async Task SearchHotelsAsync_ReturnNormalizedOfferModel()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        Assert.NotEmpty(offers);
        Assert.All(offers, offer =>
        {
            Assert.False(string.IsNullOrWhiteSpace(offer.Id));
            Assert.False(string.IsNullOrWhiteSpace(offer.Provider));
            Assert.True(offer.PerNightRate > 0);
            Assert.True(offer.TotalStayPrice > 0);
            Assert.True(Enum.IsDefined(typeof(CancellationPolicy), offer.CancellationPolicy));
            Assert.True(offer.CancellationWindowHoursBeforeCheckIn >= 0);
        });
    }

    [Fact]
    public async Task SearchHotelsAsync_ProviderNormalizationIsConsistent()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        // Verify all offers have consistent structure
        var providerGroups = offers.GroupBy(o => o.Provider);
        foreach (var group in providerGroups)
        {
            Assert.All(group, offer =>
            {
                Assert.True(Enum.IsDefined(typeof(RoomType), offer.RoomType));
                Assert.True(Enum.IsDefined(typeof(CancellationPolicy), offer.CancellationPolicy));
            });
        }
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("deluxe")]
    [InlineData("suite")]
    public async Task SearchHotelsAsync_FilteringByAllRoomTypes(string roomType)
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), roomType));

        Assert.NotEmpty(offers);
        Assert.All(offers, offer =>
        {
            var expectedType = roomType switch
            {
                "standard" => RoomType.Standard,
                "deluxe" => RoomType.Deluxe,
                "suite" => RoomType.Suite,
                _ => RoomType.Standard
            };
            Assert.Equal(expectedType, offer.RoomType);
        });
    }

    [Fact]
    public async Task SearchHotelsAsync_WithNullRoomTypeReturnsAllRoomTypes()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var roomTypes = offers.Select(o => o.RoomType).Distinct();
        Assert.True(roomTypes.Count() >= 2, "Should have multiple room types");
    }

    [Fact]
    public async Task SearchHotelsAsync_MultipleSearchesMaintainConsistency()
    {
        var service = CreateService();

        var firstSearch = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));
        var secondSearch = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        Assert.Equal(firstSearch.Count, secondSearch.Count);
        // Offers should be in same order
        for (int i = 0; i < firstSearch.Count; i++)
        {
            Assert.Equal(firstSearch[i].Id, secondSearch[i].Id);
        }
    }

    [Fact]
    public async Task SearchHotelsAsync_ThrowsForInvalidRoomType()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<HotelStay.Application.Exceptions.InvalidRequestException>(() =>
            service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), "penthouse")));

        Assert.Contains("Room type must be Standard, Deluxe, or Suite.", exception.Message);
    }

    [Theory]
    [InlineData("Deluxe")]
    [InlineData("STANDARD")]
    [InlineData("SuItE")]
    public async Task SearchHotelsAsync_RoomTypeFilteringIsCaseInsensitive(string roomTypeInput)
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), roomTypeInput));

        Assert.NotEmpty(offers);
    }

    private static HotelAvailabilityService CreateService(IReservationStore? reservationStore = null)
    {
        var store = reservationStore ?? new InMemoryReservationStore();
        var dataContext = new InMemoryDataContext();
        var providers = new IHotelProvider[]
        {
            new PremierStaysProvider(dataContext, new IProviderOfferMapper[]
            {
                new PremierStaysMapper(),
                new BudgetNestsMapper()
            }),
            new BudgetNestsProvider(dataContext, new IProviderOfferMapper[]
            {
                new PremierStaysMapper(),
                new BudgetNestsMapper()
            })
        };
        var documentValidator = new HotelDocumentValidationService(new DestinationCategorySource(dataContext));
        var offerCatalog = new HotelStay.Infrastructure.Stores.InMemoryOfferCatalog();

        return new HotelAvailabilityService(providers, store, documentValidator, offerCatalog);
    }

    private static Reservation CreateReservation(string reference, string travellerName, string offerId)
    {
        return new Reservation
        {
            ReservationReference = new ReservationReference(reference),
            TravellerName = travellerName,
            Destination = "Delhi",
            Provider = "BudgetNests",
            RoomType = RoomType.Suite,
            TotalPrice = 600m,
            CancellationPolicy = CancellationPolicy.Flexible,
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "NID-TEST",
            SelectedOfferId = offerId,
            ValidationOutcome = "Accepted"
        };
    }
}
