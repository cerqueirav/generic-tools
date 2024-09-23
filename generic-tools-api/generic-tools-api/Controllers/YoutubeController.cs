using GenericToolsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YoutubeController : ControllerBase
    {
        private readonly YoutubeService _youtubeService;

        /// <summary>
        /// Construtor do controlador que inicializa o serviço do YouTube.
        /// </summary>
        public YoutubeController()
        {
            _youtubeService = new YoutubeService();
        }

        /// <summary>
        /// Busca um vídeo pelo título.
        /// </summary>
        /// <param name="titulo">Título do vídeo a ser buscado.</param>
        /// <returns>Informações do vídeo encontrado.</returns>
        [HttpGet("buscar-video")]
        public async Task<IActionResult> BuscarVideo([FromQuery] string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return BadRequest("O título do vídeo é obrigatório.");
            }

            try
            {
                var video = await _youtubeService.BuscarVideoPorTitulo(titulo);
                if (video == null)
                {
                    return NotFound("Nenhum vídeo encontrado.");
                }

                return Ok(new
                {
                    VideoId = video.Id,
                    Titulo = video.Title,
                    Link = $"https://www.youtube.com/watch?v={video.Id}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar vídeo: {ex.Message}");
            }
        }

        /// <summary>
        /// Baixa múltiplas músicas.
        /// </summary>
        /// <param name="titulos">Lista de títulos das músicas a serem baixadas.</param>
        /// <returns>Lista de arquivos baixados.</returns>
        [HttpPost("baixar-musicas")]
        public async Task<IActionResult> BaixarMusicas([FromBody] List<string> titulos)
        {
            if (titulos == null || !titulos.Any())
            {
                return BadRequest("A lista de títulos de músicas é obrigatória.");
            }

            var savedFiles = new List<string>();
            foreach (var titulo in titulos)
            {
                var video = await _youtubeService.BuscarVideoPorTitulo(titulo);
                if (video != null)
                {
                    var filePath = await _youtubeService.BaixarArquivo(video.Id, "mp3");
                    savedFiles.Add(filePath);
                }
            }

            return Ok(new { Message = "Músicas baixadas com sucesso.", Arquivos = savedFiles });
        }

        /// <summary>
        /// Baixa um vídeo pelo título.
        /// </summary>
        /// <param name="titulo">Título do vídeo a ser baixado.</param>
        /// <returns>Arquivo do vídeo baixado.</returns>
        [HttpPost("baixar-video-por-titulo")]
        public async Task<IActionResult> BaixarVideoPorTitulo([FromBody] string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return BadRequest("O título do vídeo é obrigatório.");
            }

            try
            {
                var video = await _youtubeService.BuscarVideoPorTitulo(titulo);
                if (video == null)
                {
                    return NotFound("Vídeo não encontrado.");
                }

                var filePath = await _youtubeService.BaixarArquivo(video.Id, "mp4");
                return Ok(new
                {
                    Message = "Vídeo baixado com sucesso.",
                    Arquivo = filePath,
                    Titulo = video.Title,
                    Link = $"https://www.youtube.com/watch?v={video.Id}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao baixar vídeo: {ex.Message}");
            }
        }

        /// <summary>
        /// Baixa um vídeo pela URL.
        /// </summary>
        /// <param name="url">URL do vídeo a ser baixado.</param>
        /// <returns>Arquivo do vídeo baixado.</returns>
        [HttpPost("baixar-video-por-url")]
        public async Task<IActionResult> BaixarVideoPorUrl([FromBody] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest("A URL do vídeo é obrigatória.");
            }

            try
            {
                var videoId = _youtubeService.ExtrairVideoId(url);
                if (videoId == null)
                {
                    return BadRequest("URL inválida.");
                }

                var video = await _youtubeService.BuscarVideoPorTitulo(videoId);
                var filePath = await _youtubeService.BaixarArquivo(video.Id, "mp4");
                return Ok(new
                {
                    Message = "Vídeo baixado com sucesso.",
                    Arquivo = filePath,
                    Titulo = video.Title,
                    Link = $"https://www.youtube.com/watch?v={video.Id}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao baixar vídeo: {ex.Message}");
            }
        }

        /// <summary>
        /// Baixa uma música pela URL.
        /// </summary>
        /// <param name="url">URL da música a ser baixada.</param>
        /// <returns>Arquivo da música baixada.</returns>
        [HttpPost("baixar-musica-por-url")]
        public async Task<IActionResult> BaixarMusicaPorUrl([FromBody] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest("A URL da música é obrigatória.");
            }

            try
            {
                var videoId = _youtubeService.ExtrairVideoId(url);
                if (videoId == null)
                {
                    return BadRequest("URL inválida.");
                }

                var video = await _youtubeService.BuscarVideoPorTitulo(videoId);
                var filePath = await _youtubeService.BaixarArquivo(video.Id, "mp3");
                return Ok(new
                {
                    Message = "Música baixada com sucesso.",
                    Arquivo = filePath,
                    Titulo = video.Title,
                    Link = $"https://www.youtube.com/watch?v={video.Id}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao baixar música: {ex.Message}");
            }
        }
    }
}
