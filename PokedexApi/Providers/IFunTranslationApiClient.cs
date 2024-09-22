namespace PokedexAPI_.Providers;

public interface IFunTranslationApiClient
{
    public Task<string> GetYodaTranslationAsync(string sourceDescription);

    public Task<string> GetShakespeareTranslationAsync(string sourceDescription);
}