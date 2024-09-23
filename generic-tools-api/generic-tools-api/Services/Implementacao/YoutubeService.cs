using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using NAudio.Lame;
using NAudio.Wave;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using System.Text.RegularExpressions;
using GenericToolsAPI.Services.Interfaces;

namespace GenericToolsAPI.Services.Implementacao
{
    public class YoutubeService : IYoutubeService
    {
        private readonly YoutubeClient _youtube;
        private const string OutputDirectory = @"C:\arquivos_youtube_tmp";

        public YoutubeService()
        {
            _youtube = new YoutubeClient();
            Directory.CreateDirectory(OutputDirectory);
        }

        public async Task<YoutubeExplode.Videos.Video> BuscarVideoPorTitulo(string titulo)
        {
            var searchResults = await _youtube.Search.GetResultsAsync(titulo);
            var primeiroVideo = searchResults.OfType<VideoSearchResult>().FirstOrDefault();
            return primeiroVideo != null ? await _youtube.Videos.GetAsync(primeiroVideo.Id) : null;
        }

        public async Task<string> BaixarArquivo(string videoId, string format)
        {
            var video = await _youtube.Videos.GetAsync(videoId);
            var safeTitle = SanitizeFileName(video.Title);
            var filePath = Path.Combine(OutputDirectory, $"{safeTitle}.{format}");

            if (format.Equals("mp3", StringComparison.OrdinalIgnoreCase))
            {
                await BaixarEConverterParaMp3(videoId, filePath);
            }
            else
            {
                await BaixarVideoArquivo(videoId, filePath);
            }

            return filePath;
        }

        private async Task BaixarEConverterParaMp3(string videoId, string outputFilePath)
        {
            var tempMp4FilePath = Path.Combine(Path.GetTempPath(), $"{Path.GetFileNameWithoutExtension(outputFilePath)}.mp4");

            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId);
            var streamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();

            if (streamInfo == null)
            {
                throw new Exception("Nenhum stream de áudio encontrado.");
            }

            await _youtube.Videos.Streams.DownloadAsync(streamInfo, tempMp4FilePath);

            using (var reader = new MediaFoundationReader(tempMp4FilePath))
            {
                if (reader.WaveFormat.Channels == 0)
                {
                    throw new Exception("O arquivo MP4 baixado não contém áudio.");
                }
            }

            ConverterParaMp3(tempMp4FilePath, outputFilePath);
        }

        private async Task BaixarVideoArquivo(string videoId, string outputFilePath)
        {
            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId);
            var muxedStreams = streamManifest.GetMuxedStreams();

            var bestMuxedStream = muxedStreams.GetWithHighestBitrate();

            if (bestMuxedStream != null)
            {
                await _youtube.Videos.Streams.DownloadAsync(bestMuxedStream, outputFilePath);
            }
            else
            {
                throw new Exception("Nenhum stream muxed encontrado.");
            }
        }

        private void ConverterParaMp3(string inputFilePath, string outputFilePath)
        {
            using var reader = new MediaFoundationReader(inputFilePath);
            using var writer = new LameMP3FileWriter(outputFilePath, reader.WaveFormat, LAMEPreset.STANDARD);
            reader.CopyTo(writer);
        }

        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedFileName = new string(fileName.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
            return sanitizedFileName.Length > 255 ? sanitizedFileName.Substring(0, 255) : sanitizedFileName;
        }

        public string ExtrairVideoId(string url)
        {
            var regex = new Regex(@"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");
            var match = regex.Match(url);
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}