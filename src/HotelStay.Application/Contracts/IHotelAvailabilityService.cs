using HotelStay.Domain.Entities;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Application.Contracts;

public interface IHotelAvailabilityService
{
    Task<IReadOnlyList<HotelOffer>> SearchHotelsAsync(SearchCriteria criteria, CancellationToken cancellationToken = default);

    Task<Reservation> ReserveHotelAsync(ReservationRequest request, CancellationToken cancellationToken = default);

    Task<Reservation?> GetReservationAsync(string reference, CancellationToken cancellationToken = default);
}
