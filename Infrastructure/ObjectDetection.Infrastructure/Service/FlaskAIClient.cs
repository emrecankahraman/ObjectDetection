using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ObjectDetection.Application.Abstractions;
using ObjectDetection.Application.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ObjectDetection.Infrastructure.Services
{
    public class FlaskAIClient : IFlaskAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FlaskAIClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _configuration = config;
        }

        public async Task<ImageAnalysisResultDto> AnalyzeAsync(IFormFile imageFile)
        {
            var url = _configuration["AIService:Url"] ?? "http://127.0.0.1:5000/predict";
            using var content = new MultipartFormDataContent();
            using var ms = new MemoryStream();
            await imageFile.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var fileContent = new ByteArrayContent(ms.ToArray());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
            content.Add(fileContent, "image", imageFile.FileName);

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var root = JsonDocument.Parse(json).RootElement;

            var result = new ImageAnalysisResultDto();

            // Konum
            if (root.TryGetProperty("location", out var location))
            {
                if (location.TryGetProperty("latitude", out var lat) && lat.ValueKind == JsonValueKind.Number)
                    result.Latitude = lat.GetDouble();

                if (location.TryGetProperty("longitude", out var lng) && lng.ValueKind == JsonValueKind.Number)
                    result.Longitude = lng.GetDouble();

                if (location.TryGetProperty("country", out var country) && country.ValueKind == JsonValueKind.String)
                    result.Country = country.GetString();

                if (location.TryGetProperty("state", out var state) && state.ValueKind == JsonValueKind.String)
                    result.State = state.GetString();

                if (location.TryGetProperty("city", out var city) && city.ValueKind == JsonValueKind.String)
                    result.City = city.GetString();

                if (location.TryGetProperty("road", out var road) && road.ValueKind == JsonValueKind.String)
                    result.Road = road.GetString();
            }


            // Nesne Tespiti
            if (root.TryGetProperty("objects", out var detectionsJson))
            {
                foreach (var d in detectionsJson.EnumerateArray())
                {
                    result.Detections.Add(new DetectionResultDto
                    {
                        Class = d.GetProperty("class").GetString(),
                        Score = d.GetProperty("score").GetSingle(),
                        ColorName = d.GetProperty("color_name").GetString(),
                        DominantColorRgb = d.GetProperty("dominant_color_rgb").EnumerateArray().Select(x => x.GetInt32()).ToList()
                    });
                }
            }

            return result;
        }
    }
}
