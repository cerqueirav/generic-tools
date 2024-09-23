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

        [HttpPost("geolocalizacao")]
        public async Task<IActionResult> Geolocalizacao([FromBody] EnderecoRequest endereco)
        {
            var result = await _localizacaoService.ObterGeolocalizacaoAsync(endereco);
            return Ok(result);
        }

        [HttpPost("geolocalizacao-reversa")]
        public async Task<IActionResult> GeolocalizacaoReversa([FromBody] CoordenadasRequest coords)
        {
            var result = await _localizacaoService.ObterGeolocalizacaoReversaAsync(coords);
            return Ok(result);
        }

        [HttpPost("limites-cidade")]
        public async Task<IActionResult> LimitesCidade([FromBody] LimiteCidadeRequest request)
        {
            var result = await _localizacaoService.ObterLimitesCidadeAsync(request);
            return Ok(result);
        }

        [HttpPost("buscar-poi")]
        public async Task<IActionResult> BuscarPOI([FromBody] PontoInteresseRequest poiRequest)
        {
            var result = await _localizacaoService.BuscarPOIAsync(poiRequest);
            return Ok(result);
        }

        [HttpGet("ip-publico")]
        public async Task<IActionResult> ObterIpPublico()
        {
            var result = await _localizacaoService.ObterIpPublicoAsync();
            return Ok(result);
        }
    }
}
