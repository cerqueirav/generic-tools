using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Microsoft.AspNetCore.Mvc;
using GenericToolsAPI.Models;
using NAudio.Lame;
using NAudio.Wave;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YoutubeController : ControllerBase
    {
        private readonly YoutubeClient _youtube;
        private const string OutputDirectory = @"C:\arquivos_youtube_tmp";

        public YoutubeController()
        {
            _youtube = new YoutubeClient();
            Directory.CreateDirectory(OutputDirectory);
        }

        /// <summary>
        /// Baixa vídeos ou áudios do YouTube com base nas URLs fornecidas.
        /// </summary>
        /// <param name="request">Requisição contendo as URLs dos vídeos e o formato desejado (mp3 ou vídeo).</param>
        /// <returns>Uma lista de arquivos salvos com sucesso ou um erro se houver falha no processo.</returns>
        [HttpPost("baixar")]
        public async Task<IActionResult> Baixar([FromBody] VideoYoutubeRequest request)
        {
            if (request.VideoUrls == null || !request.VideoUrls.Any())
            {
                return BadRequest("A lista de URLs de vídeo é obrigatória.");
            }

            try
            {
                var savedFiles = new List<string>();

                foreach (var videoUrl in request.VideoUrls)
                {
                    try
                    {
                        var videoId = ExtrairVideoId(videoUrl);
                        var filePath = await BaixarArquivo(videoId, request.Format);

                        savedFiles.Add(filePath); 
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Erro ao baixar o vídeo: {ex.Message}");
                    }
                }

                return Ok(new { Message = "Vídeos baixados com sucesso.", Arquivos = savedFiles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao baixar os vídeos: {ex.Message}");
            }
        }

        /// <summary>
        /// Extrai o ID do vídeo da URL fornecida.
        /// </summary>
        /// <param name="url">URL completa do vídeo do YouTube.</param>
        /// <returns>O ID do vídeo.</returns>
        private string ExtrairVideoId(string url)
        {
            var uri = new Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return query["v"] ?? uri.Segments.Last();
        }

        /// <summary>
        /// Baixa o vídeo ou áudio no formato solicitado.
        /// </summary>
        /// <param name="videoId">ID do vídeo do YouTube.</param>
        /// <param name="format">Formato desejado (mp3 ou vídeo).</param>
        /// <returns>O caminho do arquivo salvo.</returns>
        private async Task<string> BaixarArquivo(string videoId, string format)
        {
            var video = await _youtube.Videos.GetAsync(videoId);
            var safeTitle = SanitizeFileName(video.Title);
            var filePath = Path.Combine(OutputDirectory, $"{safeTitle}.{format}");

            if (format.Equals("mp3", StringComparison.OrdinalIgnoreCase))
            {
                await BaixarEConverterParaMp3(videoId, filePath, safeTitle);
            }
            else
            {
                await BaixarVideoArquivo(videoId, filePath);
            }

            return filePath;
        }

        /// <summary>
        /// Baixa o vídeo e converte o áudio para o formato MP3.
        /// </summary>
        /// <param name="videoId">ID do vídeo do YouTube.</param>
        /// <param name="outputFilePath">Caminho onde o arquivo MP3 será salvo.</param>
        /// <param name="safeTitle">Nome sanitizado do vídeo para evitar caracteres inválidos no nome do arquivo.</param>
        private async Task BaixarEConverterParaMp3(string videoId, string outputFilePath, string safeTitle)
        {
            var tempMp4FilePath = Path.Combine(Path.GetTempPath(), $"{safeTitle}.mp4");

            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId);
            var streamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();
            await _youtube.Videos.Streams.DownloadAsync(streamInfo, tempMp4FilePath);

            ConverterParaMp3(tempMp4FilePath, outputFilePath);
        }

        /// <summary>
        /// Baixa o vídeo no formato de arquivo desejado.
        /// </summary>
        /// <param name="videoId">ID do vídeo do YouTube.</param>
        /// <param name="outputFilePath">Caminho onde o arquivo de vídeo será salvo.</param>
        private async Task BaixarVideoArquivo(string videoId, string outputFilePath)
        {
            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId);
            var streamInfo = streamManifest.GetVideoStreams().GetWithHighestBitrate();
            await _youtube.Videos.Streams.DownloadAsync(streamInfo, outputFilePath);
        }

        /// <summary>
        /// Converte o arquivo de vídeo (MP4) para MP3.
        /// </summary>
        /// <param name="inputFilePath">Caminho do arquivo MP4 temporário.</param>
        /// <param name="outputFilePath">Caminho onde o arquivo MP3 será salvo.</param>
        private void ConverterParaMp3(string inputFilePath, string outputFilePath)
        {
            using (var reader = new MediaFoundationReader(inputFilePath))
            {
                using (var writer = new LameMP3FileWriter(outputFilePath, reader.WaveFormat, LAMEPreset.STANDARD))
                {
                    reader.CopyTo(writer);
                }
            }
        }

        /// <summary>
        /// Sanitiza o nome do arquivo, removendo caracteres inválidos.
        /// </summary>
        /// <param name="fileName">Nome original do arquivo.</param>
        /// <returns>O nome sanitizado do arquivo.</returns>
        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedFileName = new string(fileName
                .Select(c => invalidChars.Contains(c) ? ' ' : c)
                .ToArray());

            const int maxFileNameLength = 255;
            if (sanitizedFileName.Length > maxFileNameLength)
            {
                sanitizedFileName = sanitizedFileName.Substring(0, maxFileNameLength);
            }

            return sanitizedFileName;
        }
    }
}
