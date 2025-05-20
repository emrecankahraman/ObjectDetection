using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObjectDetection.Application.Abstractions;
using ObjectDetection.Presentation.Models;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ObjectDetection.Presentation.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private readonly IAnalyzeImageUseCase _analyzeImageUseCase;
        private readonly IGetUserImagesUseCase _getUserImagesUseCase;

        public ImageController(IAnalyzeImageUseCase analyzeImageUseCase,IGetUserImagesUseCase getUserImagesUseCase)
        {
            _analyzeImageUseCase = analyzeImageUseCase;
            _getUserImagesUseCase = getUserImagesUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var model = new ImageUploadViewModel
            {
                UserImages = await _getUserImagesUseCase.ExecuteAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ImageUploadViewModel model)
        {
            var newModel = new ImageUploadViewModel();

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                var extension = Path.GetExtension(model.ImageFile.FileName);

                // Dosya adını güvenli hale getir
                var safeFileName = string.Concat(fileName.Where(c => char.IsLetterOrDigit(c) || c == '_'));
                var uniqueFileName = $"{safeFileName}_{Guid.NewGuid()}{extension}";

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // AI analiz ve veritabanına kayıt
                var detections = await _analyzeImageUseCase.ExecuteAsync(model.ImageFile, uniqueFileName);

                // ViewModel doldur
                newModel.UploadedImagePath = "/uploads/" + uniqueFileName;
                newModel.Results = detections;
            }

            // ✅ Sol menü için yüklenen önceki görselleri getir
            newModel.UserImages = await _getUserImagesUseCase.ExecuteAsync();

            return View(newModel);
        }
       
        
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromServices] ISearchImageUseCase searchImageUseCase)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // using System.Security.Claims;

            var model = new ImageUploadViewModel
            {
                UserImages = await _getUserImagesUseCase.ExecuteAsync(),
                SearchQuery = query,
                SearchResults = await searchImageUseCase.ExecuteAsync(query),
                CurrentUserId = userId // ✅ EKLENDİ
            };

            return View("Upload", model);
        }

    }

}
