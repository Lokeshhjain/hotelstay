using HotelStay.Domain.Entities;

namespace HotelStay.Application.Contracts;

public interface IReservationStore
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);

    Task<Reservation?> GetAsync(string reference, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetReservedOfferIdsAsync(CancellationToken cancellationToken = default);
}
