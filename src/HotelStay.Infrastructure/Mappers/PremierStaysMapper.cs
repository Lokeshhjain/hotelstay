using HotelStay.Application.Models;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Infrastructure.Mappers;

public sealed class PremierStaysMapper : IProviderOfferMapper
{
    public string ProviderName => "PremierStays";

    public ProviderHotelOffer Map(ProviderHotelOffer source, SearchCriteria criteria)
    {
        // Simulate provider-specific mapping differences (PascalCase-style source)
        // Here we normalize provider name and room type casing as part of mapping
        return source with
        {
            ProviderName = ProviderName,
            RoomTypeCode = source.RoomTypeCode?.ToLowerInvariant() ?? source.RoomTypeCode
        };
    }
}
