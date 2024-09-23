using GenericToolsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocalizacaoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LocalizacaoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("geolocalizacao")]
        public async Task<IActionResult> Geolocalizacao([FromBody] EnderecoRequest endereco)
        {
            if (endereco == null || string.IsNullOrEmpty(endereco.Rua) || string.IsNullOrEmpty(endereco.Numero) || string.IsNullOrEmpty(endereco.Bairro))
            {
                return BadRequest("Todos os parâmetros (Rua, Número, Bairro) são obrigatórios.");
            }

            var address = $"{endereco.Rua}, {endereco.Numero}, {endereco.Bairro}, {endereco.Cidade}, {endereco.Pais}";
            var encodedAddress = Uri.EscapeDataString(address);

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "GenericToolsAPI/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&addressdetails=1";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error fetching data from OSM.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }


        [HttpPost("geolocalizacao-reversa")]
        public async Task<IActionResult> GeolocalizacaoReversa([FromBody] CoordenadasRequest coords)
        {
            if (coords.Latitude < -90 || coords.Latitude > 90 || coords.Longitude < -180 || coords.Longitude > 180)
            {
                return BadRequest("As coordenadas devem estar dentro dos limites válidos.");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "GenericToolsAPI/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var url = $"https://nominatim.openstreetmap.org/reverse?lat={coords.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={coords.Longitude.ToString(CultureInfo.InvariantCulture)}&format=json";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Erro ao consultar os dados do OSM.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
    }
}
