using PokedexAPI_.Providers;

namespace PokedexAPI_.Strategies;

public class YodaTranslationStrategy(IFunTranslationApiClient funTranslationApiClient) : ITranslationStrategy
{
    public async Task<string> TranslateDescriptionAsync(string description)
    {
        return await funTranslationApiClient.GetYodaTranslationAsync(description);
    }
}