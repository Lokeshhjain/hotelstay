namespace HotelStay.Infrastructure.Mappers;

public sealed record BudgetNestsOfferResponse(
    string offer_id,
    string provider_name,
    string room_type_code,
    decimal per_night_rate,
    string cancellation_policy,
    bool is_available,
    int cancellation_window_hours_before_check_in,
    string destination);
