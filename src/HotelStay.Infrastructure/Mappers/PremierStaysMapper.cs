using HotelStay.Application.Models;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Infrastructure.Mappers;

public sealed class PremierStaysMapper : IProviderOfferMapper
{
    public string ProviderName => "PremierStays";

    public ProviderHotelOffer Map(ProviderHotelOffer source, SearchCriteria criteria)
    {
        return source with
        {
            ProviderName = ProviderName
        };
    }
}
