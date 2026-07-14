using HotelStay.Application.Models;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Infrastructure.Mappers;

public interface IProviderOfferMapper
{
    string ProviderName { get; }

    ProviderHotelOffer Map(ProviderHotelOffer source, SearchCriteria criteria);
}
