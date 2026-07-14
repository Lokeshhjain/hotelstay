using HotelStay.Application.Models;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Infrastructure.Mappers;

public sealed class BudgetNestsMapper : IProviderOfferMapper
{
    public string ProviderName => "BudgetNests";

    public ProviderHotelOffer Map(ProviderHotelOffer source, SearchCriteria criteria)
    {
        // Simulate provider-specific mapping differences (snake_case-style source)
        // Normalize provider name and map room type code to the unified format
        return source with
        {
            ProviderName = ProviderName,
            RoomTypeCode = source.RoomTypeCode?.ToLowerInvariant() ?? source.RoomTypeCode
        };
    }
}
