using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.Domain.Entities
{
    public class BoundingBox
    {
        public Guid Id { get; set; }
        public string Label { get; set; }      
        public string? Color { get; set; }     
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Guid ImageId { get; set; }
        public Image Image { get; set; }
    }
}
