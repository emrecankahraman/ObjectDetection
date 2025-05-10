using Microsoft.EntityFrameworkCore;
using ObjectDetection.Application.Abstractions;
using ObjectDetection.Domain.Entities;
using ObjectDetection.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ObjectDetection.Persistance.Concretes
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddImageWithBoundingBoxesAsync(Image image)
        {
            await _context.Images.AddAsync(image); // BoundingBoxes zaten image ile bağlı olarak gelecek
            await _context.SaveChangesAsync();
        }

        public async Task<List<Image>> GetImagesByUserAsync(Guid userId)
        {
            return await _context.Images
                .Include(i => i.BoundingBoxes)
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }
    }
}