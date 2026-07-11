using HotelStay.Application.Contracts;

namespace HotelStay.Infrastructure;

public sealed class DestinationCategorySource : IDestinationCategorySource
{
    private readonly InMemoryDataContext _dataContext;

    public DestinationCategorySource(InMemoryDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Task<IReadOnlyCollection<string>> GetDomesticCitiesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_dataContext.DomesticCities);
    }

    public Task<IReadOnlyCollection<string>> GetInternationalCitiesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_dataContext.InternationalCities);
    }
}
