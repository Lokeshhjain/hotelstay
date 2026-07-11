namespace HotelStay.Application.Contracts;

public interface IDestinationCategorySource
{
    Task<IReadOnlyCollection<string>> GetDomesticCitiesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetInternationalCitiesAsync(CancellationToken cancellationToken = default);
}
