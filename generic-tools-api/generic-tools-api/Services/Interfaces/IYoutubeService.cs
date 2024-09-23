using YoutubeExplode.Videos;

namespace GenericToolsAPI.Services.Interfaces
{
    public interface IYoutubeService
    {
        Task<Video> BuscarVideoPorTitulo(string titulo);
        Task<string> BaixarArquivo(string videoId, string format);
        string ExtrairVideoId(string url);
    }
}
