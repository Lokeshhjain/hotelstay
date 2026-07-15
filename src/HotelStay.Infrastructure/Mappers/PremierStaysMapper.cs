using HotelStay.Application.Models;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Infrastructure.Mappers;

public sealed class PremierStaysMapper : IProviderOfferMapper<PremierStaysOfferResponse>
{
    public string ProviderName => "PremierStays";

    public ProviderHotelOffer Map(PremierStaysOfferResponse source, SearchCriteria criteria)
    {
        Enum.TryParse<CancellationPolicy>(source.CancellationPolicy, true, out var cancellationPolicy);

        return new ProviderHotelOffer(
            source.Id,
            source.ProviderName,
            source.RoomTypeCode,
            source.PerNightRate,
            cancellationPolicy,
            source.IsAvailable,
            source.CancellationWindowHoursBeforeCheckIn,
            source.Destination);
    }
}
