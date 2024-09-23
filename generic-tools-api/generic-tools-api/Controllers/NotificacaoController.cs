using Microsoft.AspNetCore.Mvc;
using GenericToolsAPI.Models;
using GenericToolsAPI.Services.Interfaces;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificacaoController : ControllerBase
    {
        private readonly INotificacaoService _notificacaoService;

        /// <summary>
        /// Construtor da classe NotificacaoController.
        /// </summary>
        /// <param name="notificacaoService">Serviço de notificação que será utilizado para enviar e-mails, SMS e WhatsApp.</param>
        public NotificacaoController(INotificacaoService notificacaoService)
        {
            _notificacaoService = notificacaoService;
        }

        /// <summary>
        /// Envia um ou mais e-mails com o conteúdo fornecido.
        /// </summary>
        /// <param name="request">Objeto contendo o assunto, mensagem e lista de e-mails para envio.</param>
        /// <returns>Um IActionResult indicando o resultado do envio do e-mail.</returns>
        [HttpPost("email/enviar")]
        public async Task<IActionResult> EnviarEmail([FromBody] EmailRequest request)
        {
            if (request == null)
                return BadRequest("Requisição inválida.");

            try
            {
                await _notificacaoService.EnviarEmailAsync(request);
                return Ok("Emails enviados com sucesso!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar email: {ex.Message}");
            }
        }

        /// <summary>
        /// Envia um SMS para o número de telefone fornecido.
        /// </summary>
        /// <param name="request">Objeto contendo o número de telefone e a mensagem a ser enviada.</param>
        /// <returns>Um IActionResult indicando o resultado do envio do SMS.</returns>
        [HttpPost("sms/enviar")]
        public async Task<IActionResult> EnviarSms([FromBody] SmsRequest request)
        {
            if (request == null)
                return BadRequest("Requisição inválida.");

            try
            {
                await _notificacaoService.EnviarSmsAsync(request);
                return Ok("SMS enviado com sucesso!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar SMS: {ex.Message}");
            }
        }

        /// <summary>
        /// Envia uma mensagem via WhatsApp para o número de telefone fornecido.
        /// </summary>
        /// <param name="request">Objeto contendo o número de telefone e a mensagem a ser enviada.</param>
        /// <returns>Um IActionResult indicando o resultado do envio da mensagem via WhatsApp.</returns>
        [HttpPost("whatsapp/enviar")]
        public async Task<IActionResult> EnviarWhatsApp([FromBody] WhatsAppRequest request)
        {
            if (request == null)
                return BadRequest("Requisição inválida.");

            try
            {
                await _notificacaoService.EnviarWhatsAppAsync(request);
                return Ok("Mensagem enviada com sucesso pelo WhatsApp!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar mensagem pelo WhatsApp: {ex.Message}");
            }
        }
    }
}
