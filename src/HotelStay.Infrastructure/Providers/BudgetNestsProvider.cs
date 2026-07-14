using HotelStay.Application.Contracts;
using HotelStay.Application.Models;
using HotelStay.Domain.Services;
using HotelStay.Infrastructure.Mappers;

namespace HotelStay.Infrastructure.Providers;

public sealed class BudgetNestsProvider : IHotelProvider
{
    private readonly InMemoryDataContext _dataContext;
    private readonly IProviderOfferMapper _mapper;

    public BudgetNestsProvider(InMemoryDataContext dataContext, IEnumerable<IProviderOfferMapper> mappers)
    {
        _dataContext = dataContext;
        _mapper = mappers.Single(x => x.ProviderName == Name);
    }

    public string Name => "BudgetNests";

    public Task<IReadOnlyCollection<ProviderHotelOffer>> SearchAsync(Domain.ValueObjects.SearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        var destinationCategory = HotelBusinessRules.DetermineDestinationCategory(criteria.Destination, _dataContext.DomesticCities, _dataContext.InternationalCities);
        
        var mappedOffers = _dataContext.BudgetNestsOffers
            .Where(offer => MatchesRoomType(offer, criteria.RoomType))
            .Select(offer => _mapper.Map(offer, criteria))
            .OrderBy(offer => GetDestinationSortKey(offer, criteria.Destination))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<ProviderHotelOffer>>(mappedOffers);
    }

    private static bool MatchesRoomType(ProviderHotelOffer offer, string? roomType)
    {
        if (string.IsNullOrWhiteSpace(roomType))
        {
            return true;
        }

        return offer.RoomTypeCode.Equals(roomType.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static int GetDestinationSortKey(ProviderHotelOffer offer, string? destination)
    {
        var normalized = (destination ?? string.Empty).Trim().ToLowerInvariant();
        var hash = 0;

        foreach (var character in normalized)
        {
            hash = (hash * 31) + character;
        }

        return hash + offer.RoomTypeCode.Length + offer.Id.Length;
    }
}
