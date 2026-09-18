using BelgeArsivlemeSistemi.Models;
using OpenCvSharp;
using System.Text;

namespace BelgeArsivlemeSistemi.ImageProcessing
{
    public class DocumentEnhancer
    {
        private readonly DocumentAnalyzer analyzer = new DocumentAnalyzer();
        private readonly ImageEditor editor = new ImageEditor();

        public Mat Enhance(Mat kaynak, out string rapor)
        {
            DocumentAnalysisResult analiz = analyzer.Analyze(kaynak);

            Mat sonuc = kaynak.Clone();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Belge Analizi");
            sb.AppendLine($"Parlaklık: {analiz.BrightnessStatus}");
            sb.AppendLine($"Kontrast: {analiz.ContrastStatus}");
            sb.AppendLine($"Keskinlik: {analiz.BlurStatus}");
            sb.AppendLine();

            if (analiz.ContrastStatus == "Düşük" || analiz.BlurStatus == "Düşük")
            {
                sonuc = editor.PrepareForOCR(sonuc);
                sb.AppendLine("✓ OCR için okunabilir belge görünümü oluşturuldu.");
            }

            if (analiz.BrightnessStatus == "Düşük")
            {
                sonuc = GammaCorrection(sonuc, 1.3);
                sb.AppendLine("✓ Parlaklık iyileştirildi.");
            }

            if (sb.ToString().EndsWith("\n\n"))
            {
                sb.AppendLine("Belge zaten yeterli kalitede görünüyor.");
            }

            rapor = sb.ToString();

            return sonuc;
        }

        private Mat GammaCorrection(Mat kaynak, double gamma)
        {
            Mat sonuc = new Mat();

            Mat lookUpTable = new Mat(1, 256, MatType.CV_8U);

            for (int i = 0; i < 256; i++)
            {
                byte value = (byte)(System.Math.Pow(i / 255.0, 1.0 / gamma) * 255.0);
                lookUpTable.Set(0, i, value);
            }

            Cv2.LUT(kaynak, lookUpTable, sonuc);

            return sonuc;
        }
    }
}