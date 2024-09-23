using System.Dynamic;

namespace GenericToolsAPI.Services.Interfaces
{
    public interface IConversorService
    {
        Task<string> ConverterExcelParaSql(IFormFile file, string tableName);
        byte[] ConverterListaParaExcel(List<ExpandoObject> objects);
    }
}
