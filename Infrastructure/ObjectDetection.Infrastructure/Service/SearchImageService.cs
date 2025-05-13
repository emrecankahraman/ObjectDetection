using ObjectDetection.Application.Abstractions;
using ObjectDetection.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.Infrastructure.Service
{
    public class SearchImageService : ISearchImageUseCase
    {
        private readonly IElasticImageService _elasticService;

        public SearchImageService(IElasticImageService elasticService)
        {
            _elasticService = elasticService;
        }

        public async Task<List<ElasticImageDto>> ExecuteAsync(string keyword)
        {
            return await _elasticService.SearchAsync(keyword);
        }
    }
}
