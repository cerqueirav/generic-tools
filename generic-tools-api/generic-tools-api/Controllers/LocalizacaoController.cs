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

        /// <summary>
        /// Obt�m a geolocaliza��o a partir do endere�o fornecido (Rua, N�mero, Bairro, Cidade, Pa�s).
        /// </summary>
        [HttpPost("geolocalizacao")]
        public async Task<IActionResult> Geolocalizacao([FromBody] EnderecoRequest endereco)
        {
            if (endereco == null || string.IsNullOrEmpty(endereco.Rua) || string.IsNullOrEmpty(endereco.Numero) || string.IsNullOrEmpty(endereco.Bairro))
            {
                return BadRequest("Todos os par�metros (Rua, N�mero, Bairro) s�o obrigat�rios.");
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

        /// <summary>
        /// Realiza a geolocaliza��o reversa a partir das coordenadas fornecidas (Latitude, Longitude).
        /// </summary>
        [HttpPost("geolocalizacao-reversa")]
        public async Task<IActionResult> GeolocalizacaoReversa([FromBody] CoordenadasRequest coords)
        {
            if (coords.Latitude < -90 || coords.Latitude > 90 || coords.Longitude < -180 || coords.Longitude > 180)
            {
                return BadRequest("As coordenadas devem estar dentro dos limites v�lidos.");
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

        /// <summary>
        /// Obt�m os limites geogr�ficos da cidade com base no nome da cidade, estado e pa�s fornecidos.
        /// </summary>
        [HttpPost("limites-cidade")]
        public async Task<IActionResult> LimitesCidade([FromBody] LimiteCidadeRequest request)
        {
            if (string.IsNullOrEmpty(request.Cidade) || string.IsNullOrEmpty(request.Estado) || string.IsNullOrEmpty(request.Pais))
            {
                return BadRequest("Todos os par�metros (Cidade, Estado, Pa�s) s�o obrigat�rios.");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "GenericToolsAPI/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var query = $"{Uri.EscapeDataString(request.Cidade)}, {Uri.EscapeDataString(request.Estado)}, {Uri.EscapeDataString(request.Pais)}";
            var url = $"https://nominatim.openstreetmap.org/search?q={query}&format=json&addressdetails=1&limit=1";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Erro ao consultar os dados do OSM.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        /// <summary>
        /// Busca Pontos de Interesse (POI) com base no tipo, cidade, estado e pa�s fornecidos.
        /// </summary>
        [HttpPost("buscar-poi")]
        public async Task<IActionResult> BuscarPOI([FromBody] PontoInteresseRequest poiRequest)
        {
            if (string.IsNullOrEmpty(poiRequest.TipoPOI) || string.IsNullOrEmpty(poiRequest.Cidade) || string.IsNullOrEmpty(poiRequest.Estado) || string.IsNullOrEmpty(poiRequest.Pais))
            {
                return BadRequest("Os campos de POI, cidade, estado e pa�s s�o obrigat�rios.");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "GenericToolsAPI/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var query = $"{Uri.EscapeDataString(poiRequest.TipoPOI)} in {Uri.EscapeDataString(poiRequest.Cidade)}, {Uri.EscapeDataString(poiRequest.Estado)}, {Uri.EscapeDataString(poiRequest.Pais)}";
            var url = $"https://nominatim.openstreetmap.org/search?q={query}&format=json";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Erro ao consultar os dados do OSM.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        /// <summary>
        /// Obt�m o IP p�blico do usu�rio.
        /// </summary>
        [HttpGet("ip-publico")]
        public async Task<IActionResult> ObterIpPublico()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "GenericToolsAPI/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await client.GetAsync("https://api.ipify.org?format=json");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Erro ao obter o IP p�blico.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
    }
}
