using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Domain.Services;

public static class HotelBusinessRules
{
    public static void ValidateSearchCriteria(SearchCriteria criteria)
    {
        if (string.IsNullOrWhiteSpace(criteria.Destination))
        {
            throw new ArgumentException("Destination is required.", nameof(criteria));
        }

        if (criteria.CheckOut <= criteria.CheckIn)
        {
            throw new ArgumentException("Check-out date must be after check-in date.", nameof(criteria));
        }
    }

    public static DestinationCategory DetermineDestinationCategory(string destination, IReadOnlyCollection<string> domesticCities, IReadOnlyCollection<string> internationalCities)
    {
        var normalized = destination.Trim().ToLowerInvariant();

        // Check for exact match in domestic cities
        if (domesticCities.Any(city => normalized == city))
        {
            return DestinationCategory.Domestic;
        }

        // Check for exact match in international cities
        if (internationalCities.Any(city => normalized == city))
        {
            return DestinationCategory.International;
        }

        // Destination not found in either list
        var allCities = string.Join(", ", domesticCities.Concat(internationalCities).OrderBy(c => c));
        throw new ArgumentException($"\"{destination}\" is not a recognized destination. Valid destinations: {allCities}", nameof(destination));
    }

    public static RoomType MapRoomType(string? roomType)
    {
        return roomType?.Trim().ToLowerInvariant() switch
        {
            "suite" => RoomType.Suite,
            "deluxe" => RoomType.Deluxe,
            _ => RoomType.Standard
        };
    }

    public static bool TryParseRequestedRoomType(string? roomType, out RoomType parsedRoomType)
    {
        var normalized = roomType?.Trim().ToLowerInvariant();

        parsedRoomType = normalized switch
        {
            null or "" => default,
            "standard" => RoomType.Standard,
            "deluxe" => RoomType.Deluxe,
            "suite" => RoomType.Suite,
            _ => default
        };

        return normalized is null or "" || normalized is "standard" or "deluxe" or "suite";
    }
}
