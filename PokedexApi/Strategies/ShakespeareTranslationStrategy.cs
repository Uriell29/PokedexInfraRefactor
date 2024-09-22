using PokedexAPI_.Providers;

namespace PokedexAPI_.Strategies;

public class ShakespeareTranslationStrategy(IFunTranslationApiClient funTranslationApiClient) : ITranslationStrategy
{
    public async Task<string> TranslateDescriptionAsync(string description)
    {
        return await funTranslationApiClient.GetShakespeareTranslationAsync(description);
    }
}