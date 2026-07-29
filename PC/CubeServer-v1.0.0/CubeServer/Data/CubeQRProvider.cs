using TwoFactorAuthNet.Providers.Qr;
using QRCoder;
using System.Drawing;

namespace CubeServer.Data
{
    public class CubeQRProvider : IQrCodeProvider
    {
        QRCodeGenerator qrGen;

        public CubeQRProvider()
        {
            qrGen = new QRCodeGenerator();
        }

        public string GetMimeType()
        {
            return "image/png";
        }

        public byte[] GetQrCodeImage(string text, int size)
        {
            QRCodeData qrCodeData = qrGen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] bytes = qrCode.GetGraphic(20);

            return bytes;
        }
    }
}
