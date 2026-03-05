using System.Text;
using System.Text.Json;
using StudyMate.API.Interfaces;

namespace StudyMate.API.Services;

public class GeminiSummaryService : IAiSummaryService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public GeminiSummaryService(
        HttpClient http,
        IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<string> GenerateSummaryAsync(string text)
    {
        var apiKey = _config["Gemini:ApiKey"];

        var url =
            $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={apiKey}";

        var prompt =
            $"Summarize the following lecture clearly and concisely:\n\n{text}";

        var body = new
        {
            contents = new[]
            {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        }
        };

        var json = JsonSerializer.Serialize(body);

        var response = await _http.PostAsync(
            url,
            new StringContent(json, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API error: {error}");
        }

        var result = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(result);

        var summary =
            doc.RootElement
               .GetProperty("candidates")[0]
               .GetProperty("content")
               .GetProperty("parts")[0]
               .GetProperty("text")
               .GetString();

        return summary ?? "";
    }

}