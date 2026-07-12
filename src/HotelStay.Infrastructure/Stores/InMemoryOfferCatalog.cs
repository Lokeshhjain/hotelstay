using System.Collections.Concurrent;
using HotelStay.Application.Contracts;
using HotelStay.Domain.Entities;

namespace HotelStay.Infrastructure.Stores;

public sealed class InMemoryOfferCatalog : IOfferCatalog
{
    private readonly ConcurrentDictionary<string, HotelOffer> _offers = new(StringComparer.OrdinalIgnoreCase);

    public Task StoreAsync(IEnumerable<HotelOffer> offers, CancellationToken cancellationToken = default)
    {
        foreach (var offer in offers)
        {
            _offers[offer.Id] = offer;
        }

        return Task.CompletedTask;
    }

    public Task<HotelOffer?> GetAsync(string offerId, CancellationToken cancellationToken = default)
    {
        _offers.TryGetValue(offerId, out var offer);
        return Task.FromResult(offer);
    }
}
