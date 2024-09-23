using GenericToolsAPI.Models;

namespace GenericToolsAPI.Services.Interfaces
{
    public interface ILocalizacaoService
    {
        Task<string> ObterGeolocalizacaoAsync(EnderecoRequest endereco);
        Task<string> ObterGeolocalizacaoReversaAsync(CoordenadasRequest coords);
        Task<string> ObterLimitesCidadeAsync(LimiteCidadeRequest request);
        Task<string> BuscarPOIAsync(PontoInteresseRequest poiRequest);
        Task<string> ObterIpPublicoAsync();
    }
}
