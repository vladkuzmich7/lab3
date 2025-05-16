using SchoolRecordsSystem.Core;
using SchoolRecordsSystem.Storage;
using System.Text.Json;

namespace SchoolRecordsSystem.Logic
{

    public class Quote : IInspirationProvider
    {
        private readonly HttpClient _httpClient;

        public Quote(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public InspirationalQuote GetRandomQuote()
        {
            try
            {
                var response = _httpClient.GetAsync("https://api.breakingbadquotes.xyz/v1/quotes").Result;
                response.EnsureSuccessStatusCode();

                var json = response.Content.ReadAsStringAsync().Result;

                // Десериализуем как массив
                var quotes = JsonSerializer.Deserialize<List<BreakingBadQuote>>(json);

                if (quotes == null || quotes.Count == 0)
                    return GetDefaultQuote();

                var firstQuote = quotes[0];
                return new InspirationalQuote
                {
                    Text = firstQuote.quote,
                    Author = firstQuote.author
                };
            }
            catch
            {
                return GetDefaultQuote();
            }
        }

        private InspirationalQuote GetDefaultQuote() => new InspirationalQuote
        {
            Text = "Образование — самое мощное оружие, которое можно использовать, чтобы изменить мир.",
            Author = "Нельсон Мандела"
        };

        // Вспомогательный класс для десериализации
        private class BreakingBadQuote
        {
            public string quote { get; set; }
            public string author { get; set; }
        }
    }
}