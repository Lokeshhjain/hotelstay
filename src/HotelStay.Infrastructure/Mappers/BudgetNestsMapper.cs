using HotelStay.Application.Models;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Infrastructure.Mappers;

public sealed class BudgetNestsMapper : IProviderOfferMapper<BudgetNestsOfferResponse>
{
    public string ProviderName => "BudgetNests";

    public ProviderHotelOffer Map(BudgetNestsOfferResponse source, SearchCriteria criteria)
    {
        Enum.TryParse<CancellationPolicy>(source.cancellation_policy, true, out var cancellationPolicy);

        return new ProviderHotelOffer(
            source.offer_id,
            source.provider_name,
            source.room_type_code,
            source.per_night_rate,
            cancellationPolicy,
            source.is_available,
            source.cancellation_window_hours_before_check_in,
            source.destination);
    }
}
