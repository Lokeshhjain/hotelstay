using System.Collections.Concurrent;
using HotelStay.Application.Contracts;
using HotelStay.Domain.Entities;

namespace HotelStay.Infrastructure.Repositories;

public sealed class InMemoryReservationStore : IReservationStore
{
    private readonly ConcurrentDictionary<string, Reservation> _reservations = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, byte> _reservedOfferIds = new(StringComparer.OrdinalIgnoreCase);

    public Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        _reservations[reservation.ReservationReference.Value] = reservation;
        _reservedOfferIds[reservation.SelectedOfferId] = 0;
        return Task.CompletedTask;
    }

    public Task<Reservation?> GetAsync(string reference, CancellationToken cancellationToken = default)
    {
        _reservations.TryGetValue(reference, out var reservation);
        return Task.FromResult(reservation);
    }

    public Task<IReadOnlyCollection<string>> GetReservedOfferIdsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<string>>(_reservedOfferIds.Keys.ToList());
    }
}
