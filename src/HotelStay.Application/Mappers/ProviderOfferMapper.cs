using HotelStay.Application.Models;
using HotelStay.Domain.Entities;
using HotelStay.Domain.Services;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Application.Mappers;

public static class ProviderOfferMapper
{
    public static HotelOffer ToDomain(ProviderHotelOffer offer, SearchCriteria criteria)
    {
        return new HotelOffer
        {
            Id = offer.Id,
            Provider = offer.ProviderName,
            RoomType = HotelBusinessRules.MapRoomType(offer.RoomTypeCode),
            PerNightRate = offer.PerNightRate,
            TotalStayPrice = offer.PerNightRate * criteria.Nights,
            CancellationPolicy = offer.CancellationPolicy,
            CancellationWindowHoursBeforeCheckIn = offer.CancellationWindowHoursBeforeCheckIn,
            IsAvailable = offer.IsAvailable
        };
    }
}
