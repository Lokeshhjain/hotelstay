using HotelStay.Application.Models;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Infrastructure.Mappers;

public sealed class BudgetNestsMapper : IProviderOfferMapper
{
    public string ProviderName => "BudgetNests";

    public ProviderHotelOffer Map(ProviderHotelOffer source, SearchCriteria criteria)
    {
        return source with
        {
            ProviderName = ProviderName
        };
    }
}
