using GenericToolsAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConversorController : ControllerBase
    {
        private readonly IConversorService _conversorService;

        public ConversorController(IConversorService conversorService)
        {
            _conversorService = conversorService;
        }

        /// <summary>
        /// Recebe um arquivo Excel e gera um script SQL.
        /// </summary>
        /// <param name="file">Arquivo Excel a ser processado.</param>
        /// <param name="tableName">Nome da tabela para inserção dos dados.</param>
        /// <returns>Script SQL gerado a partir do arquivo.</returns>
        [HttpPost("excel-to-sql")]
        public async Task<IActionResult> GenerateSqlScript(IFormFile file, string tableName)
        {
            try
            {
                var sqlScript = await _conversorService.GenerateSqlScriptAsync(file, tableName);
                return Ok(sqlScript);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao gerar script SQL: {ex.Message}");
            }
        }
    }
}
