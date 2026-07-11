using HotelStay.Application.Models;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Application.Contracts;

public interface IHotelProvider
{
    string Name { get; }

    Task<IReadOnlyCollection<ProviderHotelOffer>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken = default);
}
