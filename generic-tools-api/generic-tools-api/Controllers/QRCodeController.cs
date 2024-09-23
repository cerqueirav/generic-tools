using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Net.Mime;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QRCodeController : ControllerBase
    {
        /// <summary>
        /// Gera um QR Code a partir do texto fornecido.
        /// </summary>
        /// <param name="text">Texto a ser codificado no QR Code.</param>
        /// <returns>Imagem do QR Code em formato PNG.</returns>
        [HttpGet("gerar-qrcode")]
        public IActionResult GerarQRCode(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest("Texto não pode ser vazio.");
            }

            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrCodeData);

                byte[] qrCodeImage = qrCode.GetGraphic(20);

                return File(qrCodeImage, MediaTypeNames.Image.Jpeg);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao gerar QR Code: {ex.Message}");
            }
        }
    }
}
