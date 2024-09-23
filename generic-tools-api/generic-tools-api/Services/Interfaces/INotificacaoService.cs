using GenericToolsAPI.Models;

namespace GenericToolsAPI.Services.Interfaces
{
    public interface INotificacaoService
    {
        Task EnviarEmailAsync(EmailRequest request);
        Task EnviarSmsAsync(SmsRequest request);
        Task EnviarWhatsAppAsync(WhatsAppRequest request);
    }
}
