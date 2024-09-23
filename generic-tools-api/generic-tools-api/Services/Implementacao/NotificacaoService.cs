using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using GenericToolsAPI.Models;
using GenericToolsAPI.Services.Interfaces;

namespace GenericToolsAPI.Services.Implementacao
{
    public class NotificacaoService : INotificacaoService
    {
        private readonly EmailSettings _emailSettings;
        private readonly TwilioSettings _twilioSettings;

        public NotificacaoService(IOptions<EmailSettings> emailSettings, IOptions<TwilioSettings> twilioSettings)
        {
            _emailSettings = emailSettings.Value;
            _twilioSettings = twilioSettings.Value;
            TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
        }

        public async Task EnviarEmailAsync(EmailRequest request)
        {
            if (request.Emails == null || request.Emails.Count == 0)
                throw new ArgumentException("A lista de emails não pode estar vazia.");

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
        }

        public async Task EnviarSmsAsync(SmsRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.To) || string.IsNullOrWhiteSpace(request.Message))
                throw new ArgumentException("O número de telefone e a mensagem não podem estar vazios.");

            var messageOptions = new CreateMessageOptions(new PhoneNumber(request.To))
            {
                From = new PhoneNumber(_twilioSettings.FromNumber),
                Body = request.Message,
            };

            await MessageResource.CreateAsync(messageOptions);
        }

        public async Task EnviarWhatsAppAsync(WhatsAppRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.To) || string.IsNullOrWhiteSpace(request.Message))
                throw new ArgumentException("O número de telefone e a mensagem não podem estar vazios.");

            var messageOptions = new CreateMessageOptions(new PhoneNumber($"whatsapp:{request.To}"))
            {
                From = new PhoneNumber($"whatsapp:{_twilioSettings.FromNumber}"),
                Body = request.Message,
            };

            await MessageResource.CreateAsync(messageOptions);
        }
    }
}
