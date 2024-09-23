namespace GenericToolsAPI.Services.Interfaces
{
    public interface IConversorService
    {
        Task<string> GenerateSqlScriptAsync(IFormFile file, string tableName);
    }
}
