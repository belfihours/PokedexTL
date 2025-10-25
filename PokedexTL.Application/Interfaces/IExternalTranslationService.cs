namespace PokedexTL.Application.Interfaces;

public interface IExternalTranslationService
{
    Task<string> GetTranslationWithYodaAsync(string text, CancellationToken cancellationToken);
    Task<string> GetTranslationWithShakespeareAsync(string text, CancellationToken cancellationToken);
}