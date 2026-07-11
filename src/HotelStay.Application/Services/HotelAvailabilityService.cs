using HotelStay.Application.Contracts;
using HotelStay.Domain.Entities;
using HotelStay.Domain.Services;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Application.Services;

public sealed class HotelAvailabilityService
{
    private readonly IReadOnlyCollection<IHotelProvider> _providers;
    private readonly IReservationStore _reservationStore;
    private readonly HotelDocumentValidationService _documentValidationService;

    public HotelAvailabilityService(IEnumerable<IHotelProvider> providers, IReservationStore reservationStore, HotelDocumentValidationService documentValidationService)
    {
        _providers = providers.ToList();
        _reservationStore = reservationStore;
        _documentValidationService = documentValidationService;
    }

    public async Task<IReadOnlyList<HotelOffer>> SearchHotelsAsync(SearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        HotelBusinessRules.ValidateSearchCriteria(criteria);

        var offers = new List<HotelOffer>();

        var reservedOfferIds = new HashSet<string>(await _reservationStore.GetReservedOfferIdsAsync(cancellationToken), StringComparer.OrdinalIgnoreCase);

        foreach (var provider in _providers)
        {
            var providerResults = await provider.SearchAsync(criteria, cancellationToken);

            foreach (var result in providerResults.Where(x => x.IsAvailable && !reservedOfferIds.Contains(x.Id)))
            {
                offers.Add(new HotelOffer
                {
                    Id = result.Id,
                    Provider = result.ProviderName,
                    RoomType = HotelBusinessRules.MapRoomType(result.RoomTypeCode),
                    PerNightRate = result.PerNightRate,
                    TotalStayPrice = result.PerNightRate * criteria.Nights,
                    CancellationPolicy = result.CancellationPolicy,
                    IsAvailable = true
                });
            }
        }

        return offers.OrderBy(x => x.TotalStayPrice).ToList();
    }

    public async Task<Reservation> ReserveHotelAsync(ReservationRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _documentValidationService.ValidateDocumentAsync(request.Destination, request.DocumentType, request.DocumentNumber, cancellationToken);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.Message);
        }

        var criteria = new SearchCriteria(request.Destination, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)), null);
        var offers = await SearchHotelsAsync(criteria, cancellationToken);
        var selectedOffer = offers.FirstOrDefault(x => x.Id == request.SelectedOfferId);

        if (selectedOffer is null)
        {
            throw new InvalidOperationException("The selected offer could not be found.");
        }

        var reservation = new Reservation
        {
            ReservationReference = new ReservationReference($"RSV-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}"),
            TravellerName = request.TravellerName,
            Destination = request.Destination,
            Provider = selectedOffer.Provider,
            RoomType = selectedOffer.RoomType,
            TotalPrice = selectedOffer.TotalStayPrice,
            CancellationPolicy = selectedOffer.CancellationPolicy,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            SelectedOfferId = selectedOffer.Id,
            ValidationOutcome = validation.Message
        };

        await _reservationStore.AddAsync(reservation, cancellationToken);
        return reservation;
    }

    public async Task<Reservation?> GetReservationAsync(string reference, CancellationToken cancellationToken = default)
    {
        return await _reservationStore.GetAsync(reference, cancellationToken);
    }
}
