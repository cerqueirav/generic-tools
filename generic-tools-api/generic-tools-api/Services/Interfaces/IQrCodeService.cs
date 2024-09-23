namespace GenericToolsAPI.Services.Interfaces
{
    public interface IQrCodeService
    {
        Task<byte[]> GerarQRCodeAsync(string text);
    }
}
