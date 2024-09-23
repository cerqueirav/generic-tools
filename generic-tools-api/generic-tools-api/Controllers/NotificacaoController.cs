using GenericToolsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace GenericToolsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificacaoController : ControllerBase
    {
        private readonly EmailSettings _emailSettings;
        private readonly TwilioSettings _twilioSettings;

        public NotificacaoController(IOptions<EmailSettings> emailSettings, IOptions<TwilioSettings> twilioSettings)
        {
            _emailSettings = emailSettings.Value;
            _twilioSettings = twilioSettings.Value;
            TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
        }

        /// <summary>
        /// Envia uma mensagem para um conjunto de e-mails com o assunto e texto fornecidos.
        /// </summary>
        /// <param name="request">Objeto contendo o assunto, mensagem e lista de e-mails para envio.</param>
        /// <returns>Um IActionResult indicando o resultado do envio do e-mail.</returns>
        [HttpPost("email/enviar")]
        public async Task<IActionResult> EnviarEmail([FromBody] EmailRequest request)
        {
            if (request.Emails == null || request.Emails.Count == 0)
            {
                return BadRequest("A lista de emails não pode estar vazia.");
            }

            try
            {
                var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.Port,
                    Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Email),
                    Subject = request.Subject,
                    Body = request.Message,
                    IsBodyHtml = false,
                };

                foreach (var email in request.Emails)
                {
                    mailMessage.To.Add(email);
                }

                await smtpClient.SendMailAsync(mailMessage);
                return Ok("Emails enviados com sucesso!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar email: {ex.Message}");
            }
        }

        /// <summary>
        /// Envia um SMS para um número de telefone fornecido.
        /// </summary>
        /// <param name="request">Objeto contendo o número de telefone e a mensagem a ser enviada.</param>
        /// <returns>Um IActionResult indicando o resultado do envio do SMS.</returns>
        [HttpPost("sms/enviar")]
        public async Task<IActionResult> EnviarSms([FromBody] SmsRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.To) || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("O número de telefone e a mensagem não podem estar vazios.");
            }

            try
            {
                var messageOptions = new CreateMessageOptions(new PhoneNumber(request.To))
                {
                    From = new PhoneNumber(_twilioSettings.FromNumber),
                    Body = request.Message,
                };

                await MessageResource.CreateAsync(messageOptions);
                return Ok("SMS enviado com sucesso!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar SMS: {ex.Message}");
            }
        }

        /// <summary>
        /// Envia uma mensagem WhatsApp para um número de telefone fornecido.
        /// </summary>
        /// <param name="request">Objeto contendo o número de telefone e a mensagem a ser enviada.</param>
        /// <returns>Um IActionResult indicando o resultado do envio do WhatsApp.</returns>
        [HttpPost("whatsapp/enviar")]
        public async Task<IActionResult> EnviarWhatsApp([FromBody] WhatsAppRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.To) || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("O número de telefone e a mensagem não podem estar vazios.");
            }

            try
            {
                var messageOptions = new CreateMessageOptions(new PhoneNumber($"whatsapp:{request.To}"))
                {
                    From = new PhoneNumber($"whatsapp:{_twilioSettings.FromNumber}"),
                    Body = request.Message,
                };

                await MessageResource.CreateAsync(messageOptions);
                return Ok("Mensagem enviada com sucesso pelo WhatsApp!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar mensagem pelo WhatsApp: {ex.Message}");
            }
        }
    }
}
