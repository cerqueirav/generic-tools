using GenericToolsAPI.Models;
using GenericToolsAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TraducaoController : ControllerBase
    {
        private readonly ITraducaoService _traducaoService;

        public TraducaoController(ITraducaoService traducaoService)
        {
            _traducaoService = traducaoService;
        }

        /// <summary>
        /// Traduz um texto utilizando a API MyMemory de tradução.
        /// </summary>
        /// <param name="request">Objeto contendo o texto a ser traduzido, idioma de origem e idioma de destino.</param>
        /// <returns>Um IActionResult com o texto traduzido ou uma mensagem de erro.</returns>
        [HttpPost("mymemory/traduzir")]
        public async Task<IActionResult> TraduzirMyMemory([FromBody] TraducaoMyMemoryRequest request)
        {
            if (string.IsNullOrEmpty(request.Texto) || string.IsNullOrEmpty(request.IdiomaDestino) || string.IsNullOrEmpty(request.IdiomaOrigem))
            {
                return BadRequest("Texto, idioma de destino e idioma de origem são obrigatórios.");
            }

            try
            {
                var translatedResponse = await _traducaoService.TraduzirMyMemory(request);
                return Ok(translatedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Traduz um texto utilizando a API Google Translate.
        /// </summary>
        /// <param name="request">Objeto contendo o texto a ser traduzido e o idioma de destino.</param>
        /// <returns>Um IActionResult com o texto traduzido ou uma mensagem de erro.</returns>
        [HttpPost("google/traduzir")]
        public async Task<IActionResult> TraduzirGoogle([FromBody] TraducaoGoogleRequest request)
        {
            if (string.IsNullOrEmpty(request.Texto) || string.IsNullOrEmpty(request.IdiomaDestino))
            {
                return BadRequest("Texto e idioma de destino são obrigatórios.");
            }

            try
            {
                var translatedResponse = await _traducaoService.TraduzirGoogle(request);
                return Ok(translatedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
