using GenericToolsAPI.Models;
using GenericToolsAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocalizacaoController : ControllerBase
    {
        private readonly ILocalizacaoService _localizacaoService;

        public LocalizacaoController(ILocalizacaoService localizacaoService)
        {
            _localizacaoService = localizacaoService;
        }

        /// <summary>
        /// Obtém a geolocalização a partir do endereço fornecido (Rua, Número, Bairro, Cidade, País).
        /// </summary>
        /// <param name="endereco">Objeto contendo os dados do endereço.</param>
        /// <returns>Os dados de geolocalização em formato JSON.</returns>
        [HttpPost("geolocalizacao")]
        public async Task<IActionResult> Geolocalizacao([FromBody] EnderecoRequest endereco)
        {
            var result = await _localizacaoService.ObterGeolocalizacaoAsync(endereco);
            return Ok(result);
        }

        /// <summary>
        /// Realiza a geolocalização reversa a partir das coordenadas fornecidas (Latitude, Longitude).
        /// </summary>
        /// <param name="coords">Objeto contendo as coordenadas (Latitude e Longitude).</param>
        /// <returns>Os dados de geolocalização em formato JSON.</returns>
        [HttpPost("geolocalizacao-reversa")]
        public async Task<IActionResult> GeolocalizacaoReversa([FromBody] CoordenadasRequest coords)
        {
            var result = await _localizacaoService.ObterGeolocalizacaoReversaAsync(coords);
            return Ok(result);
        }

        /// <summary>
        /// Obtém os limites geográficos da cidade com base no nome da cidade, estado e país fornecidos.
        /// </summary>
        /// <param name="request">Objeto contendo os dados da cidade, estado e país.</param>
        /// <returns>Os limites geográficos da cidade em formato JSON.</returns>
        [HttpPost("limites-cidade")]
        public async Task<IActionResult> LimitesCidade([FromBody] LimiteCidadeRequest request)
        {
            var result = await _localizacaoService.ObterLimitesCidadeAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Busca Pontos de Interesse (POI) com base no tipo, cidade, estado e país fornecidos.
        /// </summary>
        /// <param name="poiRequest">Objeto contendo os dados do tipo de POI, cidade, estado e país.</param>
        /// <returns>A lista de Pontos de Interesse em formato JSON.</returns>
        [HttpPost("buscar-poi")]
        public async Task<IActionResult> BuscarPOI([FromBody] PontoInteresseRequest poiRequest)
        {
            var result = await _localizacaoService.BuscarPOIAsync(poiRequest);
            return Ok(result);
        }

        /// <summary>
        /// Obtém o IP público do usuário.
        /// </summary>
        /// <returns>O IP público em formato JSON.</returns>
        [HttpGet("ip-publico")]
        public async Task<IActionResult> ObterIpPublico()
        {
            var result = await _localizacaoService.ObterIpPublicoAsync();
            return Ok(result);
        }
    }
}
