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
        /// Construtor do controlador que inicializa o servi�o do YouTube.
        /// </summary>
        public YoutubeController()
        {
            _youtubeService = new YoutubeService();
        }

        /// <summary>
        /// Busca um v�deo pelo t�tulo.
        /// </summary>
        /// <param name="titulo">T�tulo do v�deo a ser buscado.</param>
        /// <returns>Informa��es do v�deo encontrado.</returns>
        [HttpGet("buscar-video")]
        public async Task<IActionResult> BuscarVideo([FromQuery] string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return BadRequest("O t�tulo do v�deo � obrigat�rio.");
            }

            try
            {
                var video = await _youtubeService.BuscarVideoPorTitulo(titulo);
                if (video == null)
                {
                    return NotFound("Nenhum v�deo encontrado.");
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
                return StatusCode(500, $"Erro ao buscar v�deo: {ex.Message}");
            }
        }

        /// <summary>
        /// Baixa m�ltiplas m�sicas.
        /// </summary>
        /// <param name="titulos">Lista de t�tulos das m�sicas a serem baixadas.</param>
        /// <returns>Lista de arquivos baixados.</returns>
        [HttpPost("baixar-musicas")]
        public async Task<IActionResult> BaixarMusicas([FromBody] List<string> titulos)
        {
            if (titulos == null || !titulos.Any())
            {
                return BadRequest("A lista de t�tulos de m�sicas � obrigat�ria.");
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

            return Ok(new { Message = "M�sicas baixadas com sucesso.", Arquivos = savedFiles });
        }

        /// <summary>
        /// Baixa um v�deo pelo t�tulo.
        /// </summary>
        /// <param name="titulo">T�tulo do v�deo a ser baixado.</param>
        /// <returns>Arquivo do v�deo baixado.</returns>
        [HttpPost("baixar-video-por-titulo")]
        public async Task<IActionResult> BaixarVideoPorTitulo([FromBody] string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return BadRequest("O t�tulo do v�deo � obrigat�rio.");
            }

            try
            {
                var video = await _youtubeService.BuscarVideoPorTitulo(titulo);
                if (video == null)
                {
                    return NotFound("V�deo n�o encontrado.");
                }

                var filePath = await _youtubeService.BaixarArquivo(video.Id, "mp4");
                return Ok(new
                {
                    Message = "V�deo baixado com sucesso.",
                    Arquivo = filePath,
                    Titulo = video.Title,
                    Link = $"https://www.youtube.com/watch?v={video.Id}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao baixar v�deo: {ex.Message}");
            }
        }

        /// <summary>
        /// Baixa um v�deo pela URL.
        /// </summary>
        /// <param name="url">URL do v�deo a ser baixado.</param>
        /// <returns>Arquivo do v�deo baixado.</returns>
        [HttpPost("baixar-video-por-url")]
        public async Task<IActionResult> BaixarVideoPorUrl([FromBody] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest("A URL do v�deo � obrigat�ria.");
            }

            try
            {
                var videoId = _youtubeService.ExtrairVideoId(url);
                if (videoId == null)
                {
                    return BadRequest("URL inv�lida.");
                }

                var video = await _youtubeService.BuscarVideoPorTitulo(videoId);
                var filePath = await _youtubeService.BaixarArquivo(video.Id, "mp4");
                return Ok(new
                {
                    Message = "V�deo baixado com sucesso.",
                    Arquivo = filePath,
                    Titulo = video.Title,
                    Link = $"https://www.youtube.com/watch?v={video.Id}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao baixar v�deo: {ex.Message}");
            }
        }

        /// <summary>
        /// Baixa uma m�sica pela URL.
        /// </summary>
        /// <param name="url">URL da m�sica a ser baixada.</param>
        /// <returns>Arquivo da m�sica baixada.</returns>
        [HttpPost("baixar-musica-por-url")]
        public async Task<IActionResult> BaixarMusicaPorUrl([FromBody] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest("A URL da m�sica � obrigat�ria.");
            }

            try
            {
                var videoId = _youtubeService.ExtrairVideoId(url);
                if (videoId == null)
                {
                    return BadRequest("URL inv�lida.");
                }

                var video = await _youtubeService.BuscarVideoPorTitulo(videoId);
                var filePath = await _youtubeService.BaixarArquivo(video.Id, "mp3");
                return Ok(new
                {
                    Message = "M�sica baixada com sucesso.",
                    Arquivo = filePath,
                    Titulo = video.Title,
                    Link = $"https://www.youtube.com/watch?v={video.Id}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao baixar m�sica: {ex.Message}");
            }
        }
    }
}
