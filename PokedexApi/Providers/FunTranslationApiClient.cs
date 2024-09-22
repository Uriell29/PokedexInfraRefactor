using System.Text.Json;

namespace PokedexAPI_.Providers;

public class FunTranslationApiClient(ILogger<IFunTranslationApiClient> logger, HttpClient httpClient)
    : IFunTranslationApiClient
{
    private static readonly string baseUrl = "https://api.funtranslations.com/translate";

    public Task<string> GetYodaTranslationAsync(string sourceDescription)
    {
        return GetTranslationAsync("yoda", sourceDescription);
    }

    public Task<string> GetShakespeareTranslationAsync(string sourceDescription)
    {
        return GetTranslationAsync("shakespeare", sourceDescription);
    }

    private async Task<string> GetTranslationAsync(string translationType, string sourceDescription)
    {
        var requestUrl = $"{baseUrl}/{translationType}.json?text={Uri.EscapeDataString(sourceDescription)}";

        try
        {
            var response = await httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode(); 

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return GetFormattedTranslatedContent(jsonResponse);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An unexpected error occurred while getting {TranslationType} translation.",
                translationType);
            throw new FunTranslationApiException(
                $"An unexpected error occurred while getting {translationType} translation.", e);
        }
    }

    private static string GetFormattedTranslatedContent(string jsonResponse)
    {
        try
        {
            var doc = JsonDocument.Parse(jsonResponse);
            if (doc.RootElement.TryGetProperty("contents", out var contents) &&
                contents.TryGetProperty("translated", out var translatedText))
                return translatedText.GetString() ?? "Translation not available.";
            return "Translation not available.";
        }
        catch (JsonException e)
        {
            throw new FunTranslationApiException("Failed to parse the translation response.", e);
        }
    }
}