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

        [HttpPost("baixar")]
        public async Task<IActionResult> Baixar([FromBody] VideoYoutubeRequest request)
        {
            if (string.IsNullOrEmpty(request.VideoUrl))
            {
                return BadRequest("A URL do vídeo é obrigatória.");
            }

            try
            {
                var filePath = await BaixarArquivo(request);
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var mimeType = request.Format.Equals("mp3", StringComparison.OrdinalIgnoreCase) ? "audio/mpeg" : "video/mp4";

                return File(fileBytes, mimeType, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao baixar: {ex.Message}");
            }
        }

        private string ExtrairVideoId(string url)
        {
            var uri = new Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return query["v"] ?? uri.Segments.Last();
        }

        private async Task<string> BaixarArquivo(VideoYoutubeRequest request)
        {
            var videoId = ExtrairVideoId(request.VideoUrl);
            var video = await _youtube.Videos.GetAsync(videoId);
            var safeTitle = SanitizeFileName(video.Title);
            var filePath = Path.Combine(OutputDirectory, $"{safeTitle}.{request.Format}");

            if (request.Format.Equals("mp3", StringComparison.OrdinalIgnoreCase))
            {
                await BaixarEConverterParaMp3(videoId, filePath, safeTitle);
            }
            else
            {
                await BaixarVideoArquivo(videoId, filePath);
            }

            return filePath;
        }

        private async Task BaixarEConverterParaMp3(string videoId, string outputFilePath, string safeTitle)
        {
            var tempMp4FilePath = Path.Combine(Path.GetTempPath(), $"{safeTitle}.mp4");

            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId);
            var streamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();
            await _youtube.Videos.Streams.DownloadAsync(streamInfo, tempMp4FilePath);

            ConverterParaMp3(tempMp4FilePath, outputFilePath);
        }

        private async Task BaixarVideoArquivo(string videoId, string outputFilePath)
        {
            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId);
            var streamInfo = streamManifest.GetVideoStreams().GetWithHighestBitrate();
            await _youtube.Videos.Streams.DownloadAsync(streamInfo, outputFilePath);
        }

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
