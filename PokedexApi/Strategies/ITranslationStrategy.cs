namespace PokedexAPI_.Strategies;

public interface ITranslationStrategy
{
    Task<string> TranslateDescriptionAsync(string description);
}