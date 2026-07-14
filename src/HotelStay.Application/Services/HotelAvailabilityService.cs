using HotelStay.Application.Contracts;
using HotelStay.Application.Mappers;
using HotelStay.Domain.Entities;
using HotelStay.Domain.Services;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Application.Services;

public sealed class HotelAvailabilityService : IHotelAvailabilityService
{
    private readonly IReadOnlyCollection<IHotelProvider> _providers;
    private readonly IReservationStore _reservationStore;
    private readonly IHotelDocumentValidationService _documentValidationService;
    private readonly IOfferCatalog _offerCatalog;

    public HotelAvailabilityService(IEnumerable<IHotelProvider> providers, IReservationStore reservationStore, IHotelDocumentValidationService documentValidationService, IOfferCatalog offerCatalog)
    {
        _providers = providers.ToList();
        _reservationStore = reservationStore;
        _documentValidationService = documentValidationService;
        _offerCatalog = offerCatalog;
    }

    public async Task<IReadOnlyList<HotelOffer>> SearchHotelsAsync(SearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        HotelBusinessRules.ValidateSearchCriteria(criteria);
        var hasRequestedRoomType = HotelBusinessRules.TryParseRequestedRoomType(criteria.RoomType, out var requestedRoomType);

        if (!hasRequestedRoomType)
        {
            throw new ArgumentException("Room type must be Standard, Deluxe, or Suite.", nameof(criteria));
        }

        var offers = new List<HotelOffer>();

        var reservedOfferIds = new HashSet<string>(await _reservationStore.GetReservedOfferIdsAsync(cancellationToken), StringComparer.OrdinalIgnoreCase);

        foreach (var provider in _providers)
        {
            var providerResults = await provider.SearchAsync(criteria, cancellationToken);

            foreach (var result in providerResults.Where(x => x.IsAvailable && !reservedOfferIds.Contains(x.Id)))
            {
                offers.Add(ProviderOfferMapper.ToDomain(result, criteria));
            }
        }

        if (criteria.RoomType is not null)
        {
            offers = offers.Where(offer => offer.RoomType == requestedRoomType).ToList();
        }

        var normalizedOffers = offers.OrderBy(x => x.TotalStayPrice).ToList();
        await _offerCatalog.StoreAsync(normalizedOffers, cancellationToken);

        return normalizedOffers;
    }

    public async Task<Reservation> ReserveHotelAsync(ReservationRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _documentValidationService.ValidateDocumentAsync(request.Destination, request.DocumentType, request.DocumentNumber, cancellationToken);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.Message);
        }

        var selectedOffer = await _offerCatalog.GetAsync(request.SelectedOfferId, cancellationToken);

        if (selectedOffer is null)
        {
            throw new InvalidOperationException("The selected offer could not be found.");
        }

        // Check if this offer has already been reserved
        var reservedOfferIds = new HashSet<string>(await _reservationStore.GetReservedOfferIdsAsync(cancellationToken), StringComparer.OrdinalIgnoreCase);
        if (reservedOfferIds.Contains(selectedOffer.Id))
        {
            throw new InvalidOperationException("This offer has already been reserved. Please select another offer.");
        }

        var reservation = ReservationMapper.ToReservation(selectedOffer, request, validation);

        await _reservationStore.AddAsync(reservation, cancellationToken);
        return reservation;
    }

    public async Task<Reservation?> GetReservationAsync(string reference, CancellationToken cancellationToken = default)
    {
        return await _reservationStore.GetAsync(reference, cancellationToken);
    }
}
