using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Text;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConversorController : ControllerBase
    {
        /// <summary>
        /// Recebe um arquivo Excel e gera um script SQL.
        /// </summary>
        /// <param name="file">Arquivo Excel a ser processado.</param>
        /// <param name="tableName">Nome da tabela para inserção dos dados.</param>
        /// <returns>Script SQL gerado a partir do arquivo.</returns>
        [HttpPost("excel-to-sql")]
        public IActionResult GenerateSqlScript(IFormFile file, string tableName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file == null || file.Length == 0)
                return BadRequest("Arquivo não pode ser vazio.");

            if (string.IsNullOrEmpty(tableName))
                return BadRequest("O nome da tabela não pode ser vazio.");

            var sqlCommands = new StringBuilder();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    var columnNames = new List<string>();
                    var columnDefinitions = new List<string>();

                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        string columnName = worksheet.Cells[1, col].Text;
                        columnNames.Add(columnName);

                        string columnType = "VARCHAR(255)";
                        if (decimal.TryParse(worksheet.Cells[2, col].Text, out _))
                            columnType = "DECIMAL(18,2)";
                        else if (DateTime.TryParse(worksheet.Cells[2, col].Text, out _))
                            columnType = "DATETIME";

                        columnDefinitions.Add($"{columnName} {columnType}");
                    }

                    var sqlCreateTableCommand = $"CREATE TABLE {tableName} ({string.Join(", ", columnDefinitions)});";
                    sqlCommands.AppendLine(sqlCreateTableCommand);

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var values = new List<string>();
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            values.Add($"'{worksheet.Cells[row, col].Text}'");
                        }

                        var sqlInsertCommand = $"INSE   RT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", values)});";
                        sqlCommands.AppendLine(sqlInsertCommand);
                    }
                }
            }

            return Ok(sqlCommands.ToString());
        }
    }
}
