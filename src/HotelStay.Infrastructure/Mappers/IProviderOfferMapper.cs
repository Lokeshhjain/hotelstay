namespace HotelStay.Infrastructure.Mappers;

public interface IProviderOfferMapper<in TSource>
{
    string ProviderName { get; }

    HotelStay.Application.Models.ProviderHotelOffer Map(TSource source, HotelStay.Domain.ValueObjects.SearchCriteria criteria);
}
