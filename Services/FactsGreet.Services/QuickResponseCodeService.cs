namespace FactsGreet.Services
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;

    using QRCoder;

    public class QuickResponseCodeService
    {
        private readonly QRCodeGenerator quickResponseCodeGenerator;

        public QuickResponseCodeService(QRCodeGenerator quickResponseCodeGenerator)
        {
            this.quickResponseCodeGenerator = quickResponseCodeGenerator;
        }

        public string GetQuickResponseCodeImageSource(string data)
        {
            var quickResponseCodeData = this.quickResponseCodeGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var quickResponseCode = new QRCode(quickResponseCodeData);
            var quickResponseCodeImage = quickResponseCode.GetGraphic(20);
            using var stream = new MemoryStream();
            quickResponseCodeImage.Save(stream, ImageFormat.Png);
            return "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
        }
    }
}
