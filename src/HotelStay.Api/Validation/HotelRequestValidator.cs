using HotelStay.Api.Contracts;
using HotelStay.Domain.Enums;
using HotelStay.Domain.Services;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Api.Validation;

public static class HotelRequestValidator
{
    public static IReadOnlyCollection<string> ValidateSearch(SearchHotelsRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Destination))
        {
            errors.Add("Destination is required.");
        }

        if (request.CheckIn is null)
        {
            errors.Add("Check-in date is required.");
        }

        if (request.CheckOut is null)
        {
            errors.Add("Check-out date is required.");
        }

        if (request.CheckIn is not null && request.CheckOut is not null && request.CheckOut <= request.CheckIn)
        {
            errors.Add("Check-out date must be after check-in date.");
        }

        if (!HotelBusinessRules.TryParseRequestedRoomType(request.RoomType, out _))
        {
            errors.Add("Room type must be Standard, Deluxe, or Suite.");
        }

        return errors;
    }

    public static IReadOnlyCollection<string> ValidateReservation(ReserveHotelRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.TravellerName))
        {
            errors.Add("Traveller name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Destination))
        {
            errors.Add("Destination is required.");
        }

        if (string.IsNullOrWhiteSpace(request.DocumentType))
        {
            errors.Add("Document type is required.");
        }
        else if (!Enum.TryParse<DocumentType>(request.DocumentType, true, out _))
        {
            errors.Add("Document type is invalid.");
        }

        if (string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            errors.Add("Document number is required.");
        }

        if (string.IsNullOrWhiteSpace(request.SelectedOfferId))
        {
            errors.Add("Selected offer ID is required.");
        }

        return errors;
    }
}
