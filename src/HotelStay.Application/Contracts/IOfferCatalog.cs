using HotelStay.Domain.Entities;

namespace HotelStay.Application.Contracts;

public interface IOfferCatalog
{
    Task StoreAsync(IEnumerable<HotelOffer> offers, CancellationToken cancellationToken = default);

    Task<HotelOffer?> GetAsync(string offerId, CancellationToken cancellationToken = default);
}
