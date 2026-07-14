using HotelStay.Domain.Entities;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Application.Mappers;

public static class ReservationMapper
{
    public static Reservation ToReservation(
        HotelOffer selectedOffer,
        ReservationRequest request,
        DocumentValidationResult validation)
    {
        return new Reservation
        {
            ReservationReference = new ReservationReference($"RSV-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}"),
            TravellerName = request.TravellerName,
            Destination = request.Destination,
            Provider = selectedOffer.Provider,
            RoomType = selectedOffer.RoomType,
            TotalPrice = selectedOffer.TotalStayPrice,
            CancellationPolicy = selectedOffer.CancellationPolicy,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            SelectedOfferId = selectedOffer.Id,
            OfferSnapshot = new HotelOffer
            {
                Id = selectedOffer.Id,
                Provider = selectedOffer.Provider,
                RoomType = selectedOffer.RoomType,
                PerNightRate = selectedOffer.PerNightRate,
                TotalStayPrice = selectedOffer.TotalStayPrice,
                CancellationPolicy = selectedOffer.CancellationPolicy,
                CancellationWindowHoursBeforeCheckIn = selectedOffer.CancellationWindowHoursBeforeCheckIn,
                IsAvailable = selectedOffer.IsAvailable
            },
            ValidationOutcome = validation.Message
        };
    }
}
