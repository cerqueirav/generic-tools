using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;
using GenericToolsAPI.Services;
using GenericToolsAPI.Services.Interfaces;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QRCodeController : ControllerBase
    {
        private readonly IQrCodeService _qrCodeService;

        public QRCodeController(IQrCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        /// <summary>
        /// Gera um QR Code a partir do texto fornecido.
        /// </summary>
        /// <param name="text">Texto a ser codificado no QR Code.</param>
        /// <returns>Imagem do QR Code em formato PNG.</returns>
        [HttpGet("gerar-qrcode")]
        public async Task<IActionResult> GerarQRCode(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest("Texto não pode ser vazio.");
            }

            try
            {
                byte[] qrCodeImage = await _qrCodeService.GerarQRCodeAsync(text);
                return File(qrCodeImage, MediaTypeNames.Image.Jpeg);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao gerar QR Code: {ex.Message}");
            }
        }
    }
}
