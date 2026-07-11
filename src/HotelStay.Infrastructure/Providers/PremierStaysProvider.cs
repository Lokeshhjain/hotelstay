using HotelStay.Application.Contracts;
using HotelStay.Application.Models;
using HotelStay.Domain.Enums;

namespace HotelStay.Infrastructure.Providers;

public sealed class PremierStaysProvider : IHotelProvider
{
    private readonly InMemoryDataContext _dataContext;

    public PremierStaysProvider(InMemoryDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public string Name => "PremierStays";

    public Task<IReadOnlyCollection<ProviderHotelOffer>> SearchAsync(Domain.ValueObjects.SearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_dataContext.PremierStaysOffers);
    }
}
