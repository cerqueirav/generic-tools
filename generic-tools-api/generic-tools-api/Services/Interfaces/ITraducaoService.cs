using GenericToolsAPI.Models;

namespace GenericToolsAPI.Services.Interfaces
{
    public interface ITraducaoService
    {
        Task<string> TraduzirMyMemory(TraducaoMyMemoryRequest request);
        Task<string> TraduzirGoogle(TraducaoGoogleRequest request);
    }
}
