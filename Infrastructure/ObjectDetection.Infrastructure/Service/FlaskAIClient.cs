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

        public async Task<List<DetectionResultDto>> AnalyzeAsync(IFormFile imageFile)
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

            var detections = new List<DetectionResultDto>();
            foreach (var d in root.GetProperty("detections").EnumerateArray())
            {
                detections.Add(new DetectionResultDto
                {
                    Class = d.GetProperty("class").GetString(),
                    Score = d.GetProperty("score").GetSingle(),
                    DominantColor = d.GetProperty("dominant_color").GetString(),
                    KmeansColor = d.GetProperty("kmeans_color").GetString(),
                    ImageWidth = d.GetProperty("image_width").GetInt32(),
                    ImageHeight = d.GetProperty("image_height").GetInt32(),
                    BoundingBox = new BoundingBoxDto
                    {
                        Ymin = d.GetProperty("bounding_box").GetProperty("ymin").GetSingle(),
                        Xmin = d.GetProperty("bounding_box").GetProperty("xmin").GetSingle(),
                        Ymax = d.GetProperty("bounding_box").GetProperty("ymax").GetSingle(),
                        Xmax = d.GetProperty("bounding_box").GetProperty("xmax").GetSingle()
                    }
                });
            }

            return detections;
        }
    }
}
