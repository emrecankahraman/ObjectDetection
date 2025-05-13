using Nest;
using ObjectDetection.Application.Abstractions;
using ObjectDetection.Application.Models;
using ObjectDetection.Domain.Entities;

namespace ObjectDetection.Infrastructure.Services
{
    public class ElasticImageService : IElasticImageService
    {
        private readonly IElasticClient _client;

        public ElasticImageService(IElasticClient client)
        {
            _client = client;
        }

        public async Task IndexAsync(Image image)
        {
            var dto = new ElasticImageDto
            {
                Id = image.Id,
                FileName = image.FileName,
                Path = image.Path,
                UploadedAt = image.UploadedAt,
                Country = image.Country,
                State = image.State,
                City = image.City,
                Road = image.Road,
                Labels = image.BoundingBoxes.Select(b => b.Label).ToList(),
                Colors = image.BoundingBoxes.Select(b => b.Color).ToList()
            };

            await _client.IndexAsync(dto, idx => idx.Index("images"));
        }

        public async Task<List<ElasticImageDto>> SearchAsync(string keyword)
        {
            // 🔤 1. Anahtar kelimeyi normalize et (Türkçe harfleri sadeleştir)
            var normalizedKeyword = keyword
                .ToLowerInvariant()
                .Replace("ç", "c")
                .Replace("ğ", "g")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ş", "s")
                .Replace("ü", "u");

            var words = normalizedKeyword
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 🎯 2. Sorgu yapılandırma: MatchPhrase + Match + parçalı + fuzzy
            var shoulds = new List<Func<QueryContainerDescriptor<ElasticImageDto>, QueryContainer>>();

            // Anahtar kelime için tam eşleşme
            shoulds.Add(s => s.MatchPhrase(m => m
                .Field(f => f.Labels)
                .Query(normalizedKeyword)
                .Boost(3)
            ));

            shoulds.Add(s => s.Match(m => m
                .Field(f => f.Labels)
                .Query(normalizedKeyword)
                .Boost(2)
            ));

            shoulds.Add(s => s.Match(m => m
                .Field(f => f.Colors)
                .Query(normalizedKeyword)
                .Boost(1)
            ));

            // 🔁 Kelimeleri tek tek ara + fuzziness
            foreach (var word in words)
            {
                shoulds.Add(s => s.Match(m => m
                    .Field(f => f.Labels)
                    .Query(word)
                    .Fuzziness(Fuzziness.Auto)
                    .Boost(2)
                ));

                shoulds.Add(s => s.Match(m => m
                    .Field(f => f.Colors)
                    .Query(word)
                    .Fuzziness(Fuzziness.Auto)
                ));

                shoulds.Add(s => s.Match(m => m
                    .Field(f => f.City)
                    .Query(word)
                ));

                shoulds.Add(s => s.Match(m => m
                    .Field(f => f.Country)
                    .Query(word)
                ));
            }

            // 🔍 3. Elasticsearch isteği
            var response = await _client.SearchAsync<ElasticImageDto>(s => s
                .Index("images")
                .Query(q => q
                    .Bool(b => b
                        .Should(shoulds)
                        .MinimumShouldMatch(1)
                    )
                )
            );

            return response.Documents.ToList();
        }
    }
}
