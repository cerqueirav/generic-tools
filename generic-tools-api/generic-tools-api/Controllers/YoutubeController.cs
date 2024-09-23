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
        /// Baixa v�deos ou �udios do YouTube com base nas URLs fornecidas.
        /// </summary>
        /// <param name="request">Requisi��o contendo as URLs dos v�deos e o formato desejado (mp3 ou v�deo).</param>
        /// <returns>Uma lista de arquivos salvos com sucesso ou um erro se houver falha no processo.</returns>
        [HttpPost("baixar")]
        public async Task<IActionResult> Baixar([FromBody] VideoYoutubeRequest request)
        {
            if (request.VideoUrls == null || !request.VideoUrls.Any())
            {
                return BadRequest("A lista de URLs de v�deo � obrigat�ria.");
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
                        return StatusCode(500, $"Erro ao baixar o v�deo: {ex.Message}");
                    }
                }

                return Ok(new { Message = "V�deos baixados com sucesso.", Arquivos = savedFiles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao baixar os v�deos: {ex.Message}");
            }
        }

        /// <summary>
        /// Extrai o ID do v�deo da URL fornecida.
        /// </summary>
        /// <param name="url">URL completa do v�deo do YouTube.</param>
        /// <returns>O ID do v�deo.</returns>
        private string ExtrairVideoId(string url)
        {
            var uri = new Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return query["v"] ?? uri.Segments.Last();
        }

        /// <summary>
        /// Baixa o v�deo ou �udio no formato solicitado.
        /// </summary>
        /// <param name="videoId">ID do v�deo do YouTube.</param>
        /// <param name="format">Formato desejado (mp3 ou v�deo).</param>
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
        /// Baixa o v�deo e converte o �udio para o formato MP3.
        /// </summary>
        /// <param name="videoId">ID do v�deo do YouTube.</param>
        /// <param name="outputFilePath">Caminho onde o arquivo MP3 ser� salvo.</param>
        /// <param name="safeTitle">Nome sanitizado do v�deo para evitar caracteres inv�lidos no nome do arquivo.</param>
        private async Task BaixarEConverterParaMp3(string videoId, string outputFilePath, string safeTitle)
        {
            var tempMp4FilePath = Path.Combine(Path.GetTempPath(), $"{safeTitle}.mp4");

            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId);
            var streamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();
            await _youtube.Videos.Streams.DownloadAsync(streamInfo, tempMp4FilePath);

            ConverterParaMp3(tempMp4FilePath, outputFilePath);
        }

        /// <summary>
        /// Baixa o v�deo no formato de arquivo desejado.
        /// </summary>
        /// <param name="videoId">ID do v�deo do YouTube.</param>
        /// <param name="outputFilePath">Caminho onde o arquivo de v�deo ser� salvo.</param>
        private async Task BaixarVideoArquivo(string videoId, string outputFilePath)
        {
            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId);
            var streamInfo = streamManifest.GetVideoStreams().GetWithHighestBitrate();
            await _youtube.Videos.Streams.DownloadAsync(streamInfo, outputFilePath);
        }

        /// <summary>
        /// Converte o arquivo de v�deo (MP4) para MP3.
        /// </summary>
        /// <param name="inputFilePath">Caminho do arquivo MP4 tempor�rio.</param>
        /// <param name="outputFilePath">Caminho onde o arquivo MP3 ser� salvo.</param>
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
        /// Sanitiza o nome do arquivo, removendo caracteres inv�lidos.
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
