using Microsoft.AspNetCore.Http;
using ObjectDetection.Application.Models;
using System.Collections.Generic;

namespace ObjectDetection.Presentation.Models
{
    public class ImageUploadViewModel
    {
        public IFormFile ImageFile { get; set; }
        public List<DetectionResultDto> Results { get; set; } = new();
        public string UploadedImagePath { get; set; }
        public List<ImageDto> UserImages { get; set; }
        public string? SearchQuery { get; set; }
        public List<ElasticImageDto>? SearchResults { get; set; }
        public string CurrentUserId { get; set; }


    }
}
