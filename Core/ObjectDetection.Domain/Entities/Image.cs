using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.Domain.Entities
{
    public class Image
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid UserId { get; set; }
        public AppUser User { get; set; }
        public ICollection<BoundingBox> BoundingBoxes { get; set; }
    }
}
