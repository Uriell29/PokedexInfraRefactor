using System.Text.Json;

namespace PokedexAPI_.Providers;

public class FunTranslationApiClient(ILogger<IFunTranslationApiClient> logger, HttpClient httpClient)
    : IFunTranslationApiClient
{
    private static readonly string baseUrl = "https://api.funtranslations.com/translate";

    public async Task<string> GetYodaTranslationAsync(string sourceDescription)
    {
        var requestUrl = $"{baseUrl}/yoda.json?text={Uri.EscapeDataString(sourceDescription)}";

        try
        {
            var response = await httpClient.GetAsync(requestUrl);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return GetFormattedTranslatedContent(jsonResponse);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting yoda translation.");
            throw new FunTranslationApiException("An error occurred while getting yoda translation.");
        }
    }

    public async Task<string> GetShakespeareTranslationAsync(string sourceDescription)
    {
        var requestUrl = $"{baseUrl}/shakespeare.json?text={Uri.EscapeDataString(sourceDescription)}";

        try
        {
            var response = await httpClient.GetAsync(requestUrl);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return GetFormattedTranslatedContent(jsonResponse);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting shakespeare translation.");
            throw new FunTranslationApiException("An error occurred while getting shakespeare translation.");
        }
    }

    private static string GetFormattedTranslatedContent(string jsonResponse)
    {
        var doc = JsonDocument.Parse(jsonResponse);
        var root = doc.RootElement;

        if (root.TryGetProperty("contents", out var contents) &&
            contents.TryGetProperty("translated", out var translatedText))
            return translatedText.GetString();

        return "Translation not available.";
    }
}