using GenericToolsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TraducaoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly GoogleSettings _googleSettings;

        public TraducaoController(IHttpClientFactory httpClientFactory, IOptions<GoogleSettings> googleSettings)
        {
            _httpClientFactory = httpClientFactory;
            _googleSettings = googleSettings.Value;
        }

        [HttpPost("mymemory/traduzir")]
        public async Task<IActionResult> Traduzir([FromBody] TraducaoMyMemoryRequest request)
        {
            if (string.IsNullOrEmpty(request.Texto) || string.IsNullOrEmpty(request.IdiomaDestino) || string.IsNullOrEmpty(request.IdiomaOrigem))
            {
                return BadRequest("Texto, idioma de destino e idioma de origem s�o obrigat�rios.");
            }

            var client = _httpClientFactory.CreateClient();
            var requestUri = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(request.Texto)}&langpair={request.IdiomaOrigem}|{request.IdiomaDestino}";

            var response = await client.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Erro ao traduzir o texto.");
            }

            var translatedResponse = await response.Content.ReadAsStringAsync();
            return Ok(translatedResponse);
        }

        [HttpPost("google/traduzir")]
        public async Task<IActionResult> Traduzir([FromBody] TraducaoGoogleRequest request)
        {
            if (string.IsNullOrEmpty(request.Texto) || string.IsNullOrEmpty(request.IdiomaDestino))
            {
                return BadRequest("Texto e idioma de destino s�o obrigat�rios.");
            }

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

            if (response.IsSuccessStatusCode)
            {
                var translatedResponse = await response.Content.ReadAsStringAsync();
                return Ok(translatedResponse);
            }

            return StatusCode((int)response.StatusCode, "Erro ao traduzir o texto.");
        }
    }
}
