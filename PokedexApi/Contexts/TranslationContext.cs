using PokedexAPI_.Strategies;

namespace PokedexAPI_.Contexts;

public class TranslationContext
{
    private ITranslationStrategy _strategy;

    public void SetTranslationStrategy(ITranslationStrategy strategy)
    {
        _strategy = strategy;
    }

    public async Task<string> TranslateDescriptionAsync(string description)
    {
        if (_strategy == null)
            throw new InvalidOperationException("Translation strategy not set.");

        return await _strategy.TranslateDescriptionAsync(description);
    }
}