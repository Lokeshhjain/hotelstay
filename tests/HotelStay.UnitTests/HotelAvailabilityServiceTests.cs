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
using Moq;

namespace HotelStay.UnitTests;

public sealed class HotelAvailabilityServiceTests
{
    [Fact]
    public async Task SearchHotelsAsync_ReturnsNormalizedOffersSortedByTotalPrice()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        Assert.Equal(3, offers.Count);
        Assert.Collection(offers,
            first =>
            {
                Assert.Equal("premier-1", first.Id);
                Assert.Equal("PremierStays", first.Provider);
                Assert.Equal(RoomType.Standard, first.RoomType);
                Assert.Equal(360m, first.TotalStayPrice);
            },
            second =>
            {
                Assert.Equal("premier-2", second.Id);
                Assert.Equal("PremierStays", second.Provider);
                Assert.Equal(RoomType.Deluxe, second.RoomType);
                Assert.Equal(540m, second.TotalStayPrice);
            },
            third =>
            {
                Assert.Equal("budget-2", third.Id);
                Assert.Equal("BudgetNests", third.Provider);
                Assert.Equal(RoomType.Suite, third.RoomType);
                Assert.Equal(660m, third.TotalStayPrice);
            });
    }

    [Fact]
    public async Task SearchHotelsAsync_ExcludesUnavailableAndReservedOffers()
    {
        var reservationStore = new InMemoryReservationStore();
        await reservationStore.AddAsync(new Reservation
        {
            ReservationReference = new ReservationReference("RSV-RESERVED"),
            TravellerName = "Existing Guest",
            Destination = "Delhi",
            Provider = "BudgetNests",
            RoomType = RoomType.Suite,
            TotalPrice = 660m,
            CancellationPolicy = CancellationPolicy.Flexible,
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "NID-1",
            SelectedOfferId = "budget-2",
            ValidationOutcome = "Accepted"
        });

        var service = CreateService(reservationStore);

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        Assert.DoesNotContain(offers, offer => offer.Id == "budget-1");
        Assert.DoesNotContain(offers, offer => offer.Id == "budget-2");
        Assert.Equal(2, offers.Count);
    }

    [Fact]
    public async Task SearchHotelsAsync_FiltersByRequestedRoomType()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), "suite"));

        Assert.Single(offers);
        Assert.Equal("budget-2", offers[0].Id);
        Assert.Equal(RoomType.Suite, offers[0].RoomType);
    }

    [Fact]
    public async Task SearchHotelsAsync_ThrowsForInvalidRequestedRoomType()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), "penthouse")));

        Assert.Contains("Room type must be Standard, Deluxe, or Suite.", exception.Message);
    }

    [Fact]
    public async Task SearchHotelsAsync_PopulatesCancellationPolicyWindowHoursBeforeCheckIn()
    {
        var service = CreateService();

        var offers = await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var premierOffer = Assert.Single(offers, x => x.Id == "premier-1");
        var budgetOffer = Assert.Single(offers, x => x.Id == "budget-2");

        Assert.Equal(48, premierOffer.CancellationWindowHoursBeforeCheckIn);
        Assert.Equal(24, budgetOffer.CancellationWindowHoursBeforeCheckIn);
    }

    [Fact]
    public async Task BudgetNestsProvider_SearchAsync_FiltersByRequestedRoomType()
    {
        var dataContext = new InMemoryDataContext();
        var provider = new BudgetNestsProvider(dataContext, new IProviderOfferMapper[]
        {
            new PremierStaysMapper(),
            new BudgetNestsMapper()
        });

        var offers = await provider.SearchAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), "suite"));

        var offer = Assert.Single(offers);
        Assert.Equal("budget-2", offer.Id);
    }

    [Fact]
    public async Task ReserveHotelAsync_CreatesReservationForValidDocument()
    {
        var service = CreateService();

        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var reservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        Assert.NotNull(reservation.ReservationReference.Value);
        Assert.Equal("Alex", reservation.TravellerName);
        Assert.Equal("PremierStays", reservation.Provider);
        Assert.Equal("premier-1", reservation.SelectedOfferId);
        Assert.Equal(360m, reservation.TotalPrice);
        Assert.Equal("Document accepted.", reservation.ValidationOutcome);
    }

    [Fact]
    public async Task ReserveHotelAsync_ThrowsWhenDocumentIsInvalid()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReserveHotelAsync(
            new ReservationRequest(
                "Alex",
                "Paris",
                DocumentType.NationalId,
                "NID-123",
                "premier-1")));

        Assert.Equal("International destinations require a Passport.", exception.Message);
    }

    [Fact]
    public async Task GetReservationAsync_ReturnsStoredReservation()
    {
        var service = CreateService();
        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));
        var reservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        var fetched = await service.GetReservationAsync(reservation.ReservationReference.Value);

        Assert.NotNull(fetched);
        Assert.Equal(reservation.ReservationReference.Value, fetched!.ReservationReference.Value);
        Assert.Equal("Alex", fetched.TravellerName);
    }

    [Fact]
    public async Task ReserveHotelAsync_StoresFullOfferSnapshotInReservation()
    {
        var service = CreateService();
        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var reservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        Assert.NotNull(reservation.OfferSnapshot);
        Assert.Equal("premier-1", reservation.OfferSnapshot!.Id);
        Assert.Equal("PremierStays", reservation.OfferSnapshot.Provider);
        Assert.Equal(RoomType.Standard, reservation.OfferSnapshot.RoomType);
        Assert.Equal(360m, reservation.OfferSnapshot.TotalStayPrice);
    }

    private static HotelAvailabilityService CreateService(IReservationStore? reservationStore = null)
    {
        var dataContext = new InMemoryDataContext();
        var destinationSource = new DestinationCategorySource(dataContext);
        var documentValidationService = new HotelDocumentValidationService(destinationSource);
        var offerCatalog = new InMemoryOfferCatalog();
        var mappers = new IProviderOfferMapper[]
        {
            new PremierStaysMapper(),
            new BudgetNestsMapper()
        };

        var providers = new IHotelProvider[]
        {
            new PremierStaysProvider(dataContext, mappers),
            new BudgetNestsProvider(dataContext, mappers)
        };

        return new HotelAvailabilityService(providers, reservationStore ?? new InMemoryReservationStore(), documentValidationService, offerCatalog);
    }
}
