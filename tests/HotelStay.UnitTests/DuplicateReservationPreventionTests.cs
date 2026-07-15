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
/// Tests for duplicate reservation prevention and advanced reservation scenarios.
/// Ensures that the same offer cannot be reserved multiple times and that concurrent
/// reservation scenarios are handled correctly.
/// </summary>
public sealed class DuplicateReservationPreventionTests
{
    [Fact]
    public async Task ReserveHotelAsync_ThrowsWhenOfferAlreadyReserved()
    {
        var service = CreateService();
        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReserveHotelAsync(
            new ReservationRequest(
                "Bob",
                "Delhi",
                DocumentType.NationalId,
                "NID-456",
                "premier-1")));

        Assert.Equal("This offer has already been reserved. Please select another offer.", exception.Message);
    }

    [Fact]
    public async Task ReserveHotelAsync_AllowsMultipleReservationsForDifferentOffers()
    {
        var reservationStore = new InMemoryReservationStore();
        var service = CreateService(reservationStore);

        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        // Reserve first offer
        var firstReservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        // Reserve different offer - should succeed
        var secondReservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Bob",
            "Delhi",
            DocumentType.NationalId,
            "NID-456",
            "premier-2"));  // Different offer

        Assert.NotEqual(firstReservation.ReservationReference.Value, secondReservation.ReservationReference.Value);
        Assert.Equal("premier-1", firstReservation.SelectedOfferId);
        Assert.Equal("premier-2", secondReservation.SelectedOfferId);
    }

    [Fact]
    public async Task ReserveHotelAsync_DuplicateCheckIsCaseInsensitiveForOfferId()
    {
        var service = CreateService();
        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReserveHotelAsync(
            new ReservationRequest(
                "Bob",
                "Delhi",
                DocumentType.NationalId,
                "NID-456",
                "PREMIER-1")));

        Assert.Equal("This offer has already been reserved. Please select another offer.", exception.Message);
    }

    [Fact]
    public async Task ReserveHotelAsync_SameTravellerCanReserveDifferentOffers()
    {
        var reservationStore = new InMemoryReservationStore();
        var service = CreateService(reservationStore);

        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        // Same traveller reserves two different offers
        var firstReservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        var secondReservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",  // Same traveller
            "Delhi",
            DocumentType.NationalId,
            "NID-123",  // Same ID (allowed for different offers)
            "premier-2"));  // Different offer

        Assert.NotEqual(firstReservation.ReservationReference.Value, secondReservation.ReservationReference.Value);
    }

    [Fact]
    public async Task ReserveHotelAsync_PreservesReservedOfferIdsDuringMultipleReservations()
    {
        var reservationStore = new InMemoryReservationStore();
        var service = CreateService(reservationStore);

        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        // Make first reservation
        await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        // Make second reservation
        await service.ReserveHotelAsync(new ReservationRequest(
            "Bob",
            "Delhi",
            DocumentType.NationalId,
            "NID-456",
            "premier-2"));

        // Verify both offers are in the reserved set
        var reservedOfferIds = await reservationStore.GetReservedOfferIdsAsync();

        Assert.Contains("premier-1", reservedOfferIds);
        Assert.Contains("premier-2", reservedOfferIds);
        Assert.Equal(2, reservedOfferIds.Count);
    }

    [Fact]
    public async Task ReserveHotelAsync_CreatesReservationSnapshotWithCompleteOfferData()
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
        Assert.NotNull(reservation.OfferSnapshot.Id);
        Assert.NotNull(reservation.OfferSnapshot.Provider);
        Assert.Equal(RoomType.Standard, reservation.OfferSnapshot.RoomType);
        Assert.NotEqual(0m, reservation.OfferSnapshot.TotalStayPrice);
    }

    [Fact]
    public async Task ReserveHotelAsync_IncludesCancellationPolicyInReservation()
    {
        var service = CreateService();

        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var reservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        // Premier offers 48-hour cancellation
        Assert.Equal(CancellationPolicy.FreeCancellation, reservation.CancellationPolicy);
        Assert.NotNull(reservation.OfferSnapshot.CancellationWindowHoursBeforeCheckIn);
        Assert.Equal(48, reservation.OfferSnapshot.CancellationWindowHoursBeforeCheckIn);
    }

    [Fact]
    public async Task ReserveHotelAsync_RejectsInternationalDestinationWithNationalId()
    {
        var service = CreateService();
        await service.SearchHotelsAsync(new SearchCriteria("London", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReserveHotelAsync(
            new ReservationRequest(
                "Alex",
                "London",
                DocumentType.NationalId,
                "NID-123",
                "premier-1")));

        Assert.Equal("International destinations require a Passport.", exception.Message);
    }

    [Fact]
    public async Task ReserveHotelAsync_AcceptsInternationalDestinationWithPassport()
    {
        var service = CreateService();
        await service.SearchHotelsAsync(new SearchCriteria("London", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var reservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "London",
            DocumentType.Passport,
            "PASSPORT-123",
            "premier-london-1"));

        Assert.Equal("Document accepted.", reservation.ValidationOutcome);
        Assert.NotNull(reservation.ReservationReference.Value);
    }

    [Fact]
    public async Task GetReservationAsync_ReturnsReservationWithSameReferenceCase()
    {
        var service = CreateService();
        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var reservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        var reference = reservation.ReservationReference.Value;
        var fetched = await service.GetReservationAsync(reference);

        Assert.NotNull(fetched);
        Assert.Equal(reference, fetched!.ReservationReference.Value);
    }

    [Fact]
    public async Task GetReservationAsync_ReturnsCaseInsensitiveMatch()
    {
        var service = CreateService();
        await service.SearchHotelsAsync(new SearchCriteria("Delhi", new DateOnly(2026, 7, 11), new DateOnly(2026, 7, 14), null));

        var reservation = await service.ReserveHotelAsync(new ReservationRequest(
            "Alex",
            "Delhi",
            DocumentType.NationalId,
            "NID-123",
            "premier-1"));

        var reference = reservation.ReservationReference.Value;
        var lowercaseReference = reference.ToLowerInvariant();

        var fetched = await service.GetReservationAsync(lowercaseReference);

        Assert.NotNull(fetched);
    }

    [Fact]
    public async Task GetReservationAsync_ReturnsNullForNonExistentReference()
    {
        var service = CreateService();

        var fetched = await service.GetReservationAsync("RSV-NONEXISTENT");

        Assert.Null(fetched);
    }

    private static HotelAvailabilityService CreateService(IReservationStore? reservationStore = null)
    {
        var store = reservationStore ?? new InMemoryReservationStore();
        var dataContext = new InMemoryDataContext();
        var providers = new IHotelProvider[]
        {
            new PremierStaysProvider(dataContext, new IProviderOfferMapper<PremierStaysOfferResponse>[]
            {
                new PremierStaysMapper()
            }),
            new BudgetNestsProvider(dataContext, new IProviderOfferMapper<BudgetNestsOfferResponse>[]
            {
                new BudgetNestsMapper()
            })
        };
        var documentValidator = new HotelDocumentValidationService(new DestinationCategorySource(dataContext));
        var offerCatalog = new HotelStay.Infrastructure.Stores.InMemoryOfferCatalog();

        return new HotelAvailabilityService(providers, store, documentValidator, offerCatalog);
    }
}
