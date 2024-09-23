using GenericToolsAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

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
        [HttpPost("excel-para-sql")]
        public async Task<IActionResult> ConverterExcelParaSql(IFormFile file, string tableName)
        {
            try
            {
                var sqlScript = await _conversorService.ConverterExcelParaSql(file, tableName);
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

        /// <summary>
        /// Converte uma lista de objetos em um arquivo Excel.
        /// </summary>
        /// <param name="objects">Lista de objetos a serem convertidos.</param>
        /// <returns>Arquivo Excel gerado a partir da lista de objetos.</returns>
        [HttpPost("lista-para-excel")]
        public IActionResult ConverterListaParaExcel([FromBody] List<ExpandoObject> objects)
        {
            try
            {
                var excelData = _conversorService.ConverterListaParaExcel(objects);
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "lista_de_objetos.xlsx");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao gerar arquivo Excel: {ex.Message}");
            }
        }
    }
}
