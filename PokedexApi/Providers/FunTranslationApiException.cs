namespace PokedexAPI_.Providers;

public class FunTranslationApiException(string message, Exception innerException = null)
    : Exception(message, innerException);