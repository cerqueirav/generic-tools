using GenericToolsAPI.Models;
using GenericToolsAPI.Services.Interfaces;
using System.Globalization;

namespace GenericToolsAPI.Services.Implementacao
{
    public class LocalizacaoService : ILocalizacaoService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LocalizacaoService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> ObterGeolocalizacaoAsync(EnderecoRequest endereco)
        {
            var address = $"{endereco.Rua}, {endereco.Numero}, {endereco.Bairro}, {endereco.Cidade}, {endereco.Pais}";
            var encodedAddress = Uri.EscapeDataString(address);
            var client = _httpClientFactory.CreateClient();

            var url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&addressdetails=1";
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ObterGeolocalizacaoReversaAsync(CoordenadasRequest coords)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://nominatim.openstreetmap.org/reverse?lat={coords.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={coords.Longitude.ToString(CultureInfo.InvariantCulture)}&format=json";
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ObterLimitesCidadeAsync(LimiteCidadeRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var query = $"{Uri.EscapeDataString(request.Cidade)}, {Uri.EscapeDataString(request.Estado)}, {Uri.EscapeDataString(request.Pais)}";
            var url = $"https://nominatim.openstreetmap.org/search?q={query}&format=json&addressdetails=1&limit=1";
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> BuscarPOIAsync(PontoInteresseRequest poiRequest)
        {
            var client = _httpClientFactory.CreateClient();
            var query = $"{Uri.EscapeDataString(poiRequest.TipoPOI)} in {Uri.EscapeDataString(poiRequest.Cidade)}, {Uri.EscapeDataString(poiRequest.Estado)}, {Uri.EscapeDataString(poiRequest.Pais)}";
            var url = $"https://nominatim.openstreetmap.org/search?q={query}&format=json";
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ObterIpPublicoAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://api.ipify.org?format=json");

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
