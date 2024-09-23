using GenericToolsAPI.Models;
using System.Text;
using Microsoft.Extensions.Options;
using GenericToolsAPI.Services.Interfaces;

namespace GenericToolsAPI.Services.Implementacao
{
    public class TraducaoService : ITraducaoService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GoogleSettings _googleSettings;

        public TraducaoService(IHttpClientFactory httpClientFactory, IOptions<GoogleSettings> googleSettings)
        {
            _httpClientFactory = httpClientFactory;
            _googleSettings = googleSettings.Value;
        }

        public async Task<string> TraduzirMyMemory(TraducaoMyMemoryRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var requestUri = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(request.Texto)}&langpair={request.IdiomaOrigem}|{request.IdiomaDestino}";

            var response = await client.GetAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erro ao traduzir o texto com MyMemory.");
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> TraduzirGoogle(TraducaoGoogleRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var requestBody = new
            {
                q = request.Texto,
                target = request.IdiomaDestino,
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var requestUri = $"https://translation.googleapis.com/language/translate/v2?key={_googleSettings.ChaveApi}";

            var response = await client.PostAsync(requestUri, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erro ao traduzir o texto com Google Translate.");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
