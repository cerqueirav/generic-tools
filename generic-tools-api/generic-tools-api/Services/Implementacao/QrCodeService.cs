using GenericToolsAPI.Services.Interfaces;
using QRCoder;

namespace GenericToolsAPI.Services.Implementacao
{
    public class QrCodeService : IQrCodeService
    {
        public async Task<byte[]> GerarQRCodeAsync(string text)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            return await Task.FromResult(qrCode.GetGraphic(20));
        }
    }
}
