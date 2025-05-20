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
                UserId = image.UserId.ToString(),
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
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var allResults = await _client.SearchAsync<ElasticImageDto>(s => s
                    .Index("images")
                    .Query(q => q.MatchAll())
                    .Size(1000)
                );

                return allResults.Documents.ToList();
            }
                // 🔤 1. Anahtar kelimeyi normalize et (Türkçe harfleri sadeleştir)
                string NormalizeText(string input)
            {
                return input
                    .ToLowerInvariant()
                    .Replace("ç", "c")
                    .Replace("ğ", "g")
                    .Replace("ı", "i")
                    .Replace("ö", "o")
                    .Replace("ş", "s")
                    .Replace("ü", "u");
            }

            var normalizedKeyword = NormalizeText(keyword);
            var words = normalizedKeyword.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 🎯 2. Sorgu yapılandırma: MatchPhrase + Match + fuzzy
            var shoulds = new List<Func<QueryContainerDescriptor<ElasticImageDto>, QueryContainer>>();

            // Etiket ve renklerde tam eşleşme
            shoulds.Add(s => s.MatchPhrase(m => m.Field(f => f.Labels).Query(normalizedKeyword).Boost(3)));
            shoulds.Add(s => s.Match(m => m.Field(f => f.Labels).Query(normalizedKeyword).Boost(2)));
            shoulds.Add(s => s.Match(m => m.Field(f => f.Colors).Query(normalizedKeyword).Boost(1)));

            // 🔁 Kelimeleri tek tek ve farklı alanlarda fuzzy + orijinal ile eşleştir
            foreach (var word in words)
            {
                // Labels
                shoulds.Add(s => s.Match(m => m.Field(f => f.Labels).Query(word).Fuzziness(Fuzziness.Auto).Boost(2)));

                // Colors
                shoulds.Add(s => s.Match(m => m.Field(f => f.Colors).Query(word).Fuzziness(Fuzziness.Auto)));

                // City (orijinal ve normalize edilmiş)
                shoulds.Add(s => s.MatchPhrase(m => m.Field(f => f.City).Query(keyword).Boost(3)));
                shoulds.Add(s => s.Match(m => m.Field(f => f.City).Query(word).Fuzziness(Fuzziness.Auto).Boost(1.5)));

                // Country
                shoulds.Add(s => s.MatchPhrase(m => m.Field(f => f.Country).Query(keyword)));
                shoulds.Add(s => s.Match(m => m.Field(f => f.Country).Query(word).Fuzziness(Fuzziness.Auto)));
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
