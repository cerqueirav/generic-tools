using GenericToolsAPI.Services.Interfaces;
using OfficeOpenXml;
using System.Dynamic;
using System.Text;

namespace GenericToolsAPI.Services.Implementacao
{
    public class ConversorService : IConversorService
    {
        public async Task<string> ConverterExcelParaSql(IFormFile file, string tableName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file == null || file.Length == 0)
                throw new ArgumentException("Arquivo não pode ser vazio.");

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("O nome da tabela não pode ser vazio.");

            var sqlCommands = new StringBuilder();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
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

                        var sqlInsertCommand = $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", values)});";
                        sqlCommands.AppendLine(sqlInsertCommand);
                    }
                }
            }

            return sqlCommands.ToString();
        }

        public byte[] ConverterListaParaExcel(List<ExpandoObject> objects)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (objects == null || objects.Count == 0)
                throw new ArgumentException("A lista de objetos não pode ser vazia.");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                var firstObject = objects.First();
                var dictionary = firstObject as IDictionary<string, object>;
                if (dictionary == null || dictionary.Count == 0)
                    throw new ArgumentException("O tipo de objeto não possui propriedades.");

                int columnIndex = 1;
                foreach (var key in dictionary.Keys)
                {
                    worksheet.Cells[1, columnIndex++].Value = key;
                }

                for (int rowIndex = 0; rowIndex < objects.Count; rowIndex++)
                {
                    var objDict = objects[rowIndex] as IDictionary<string, object>;
                    columnIndex = 1;
                    foreach (var value in objDict.Values)
                    {
                        worksheet.Cells[rowIndex + 2, columnIndex++].Value = value;
                    }
                }

                return package.GetAsByteArray();
            }
        }
    }
}
