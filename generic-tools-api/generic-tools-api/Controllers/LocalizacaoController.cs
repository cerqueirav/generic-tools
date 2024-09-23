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
        /// Obt�m a geolocaliza��o a partir do endere�o fornecido (Rua, N�mero, Bairro, Cidade, Pa�s).
        /// </summary>
        /// <param name="endereco">Objeto contendo os dados do endere�o.</param>
        /// <returns>Os dados de geolocaliza��o em formato JSON.</returns>
        [HttpPost("geolocalizacao")]
        public async Task<IActionResult> Geolocalizacao([FromBody] EnderecoRequest endereco)
        {
            var result = await _localizacaoService.ObterGeolocalizacaoAsync(endereco);
            return Ok(result);
        }

        /// <summary>
        /// Realiza a geolocaliza��o reversa a partir das coordenadas fornecidas (Latitude, Longitude).
        /// </summary>
        /// <param name="coords">Objeto contendo as coordenadas (Latitude e Longitude).</param>
        /// <returns>Os dados de geolocaliza��o em formato JSON.</returns>
        [HttpPost("geolocalizacao-reversa")]
        public async Task<IActionResult> GeolocalizacaoReversa([FromBody] CoordenadasRequest coords)
        {
            var result = await _localizacaoService.ObterGeolocalizacaoReversaAsync(coords);
            return Ok(result);
        }

        /// <summary>
        /// Obt�m os limites geogr�ficos da cidade com base no nome da cidade, estado e pa�s fornecidos.
        /// </summary>
        /// <param name="request">Objeto contendo os dados da cidade, estado e pa�s.</param>
        /// <returns>Os limites geogr�ficos da cidade em formato JSON.</returns>
        [HttpPost("limites-cidade")]
        public async Task<IActionResult> LimitesCidade([FromBody] LimiteCidadeRequest request)
        {
            var result = await _localizacaoService.ObterLimitesCidadeAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Busca Pontos de Interesse (POI) com base no tipo, cidade, estado e pa�s fornecidos.
        /// </summary>
        /// <param name="poiRequest">Objeto contendo os dados do tipo de POI, cidade, estado e pa�s.</param>
        /// <returns>A lista de Pontos de Interesse em formato JSON.</returns>
        [HttpPost("buscar-poi")]
        public async Task<IActionResult> BuscarPOI([FromBody] PontoInteresseRequest poiRequest)
        {
            var result = await _localizacaoService.BuscarPOIAsync(poiRequest);
            return Ok(result);
        }

        /// <summary>
        /// Obt�m o IP p�blico do usu�rio.
        /// </summary>
        /// <returns>O IP p�blico em formato JSON.</returns>
        [HttpGet("ip-publico")]
        public async Task<IActionResult> ObterIpPublico()
        {
            var result = await _localizacaoService.ObterIpPublicoAsync();
            return Ok(result);
        }
    }
}
