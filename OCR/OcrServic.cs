using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tesseract;

namespace BelgeArsivlemeSistemi.OCR
{
    public sealed class OcrService : IDisposable
    {
        private readonly TesseractEngine engine;
        private bool disposed;

        private sealed class OcrSonucu
        {
            public string Metin { get; set; } = string.Empty;
            public float Guven { get; set; }
            public int AnlamliKarakterSayisi { get; set; }
            public int AnahtarKelimePuani { get; set; }
            public int BozukKarakterSayisi { get; set; }

            public double Skor =>
                Guven * 100.0 +
                AnahtarKelimePuani * 20.0 +
                Math.Min(AnlamliKarakterSayisi, 300) * 0.02 -
                BozukKarakterSayisi * 0.5;
        }

        private int AnahtarKelimePuaniHesapla(string metin)
        {
            if (string.IsNullOrWhiteSpace(metin))
                return 0;

            string temiz = metin
                .ToUpperInvariant()
                .Replace('İ', 'I')
                .Replace('Ş', 'S')
                .Replace('Ğ', 'G')
                .Replace('Ü', 'U')
                .Replace('Ö', 'O')
                .Replace('Ç', 'C');

            string[] anahtarKelimeler =
            {
        "TURKIYE",
        "CUMHURIYETI",
        "KIMLIK",
        "SOYADI",
        "DOGUM",
        "IHALE",
        "BELEDIYE",
        "EDIRNE",
        "KESAN",
        "RUHSAT",
        "FATURA",
        "ADRES",
        "TARIH",
        "PARSEL",
        "MUDURLUGU",
        "BASKANLIGI"
    };

            int puan = 0;

            foreach (string kelime in anahtarKelimeler)
            {
                if (temiz.Contains(kelime))
                    puan++;
            }

            return puan;
        }
        public OcrService()
        {
            string tessPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "tessdata");

            if (!Directory.Exists(tessPath))
            {
                throw new DirectoryNotFoundException(
                    $"tessdata klasörü bulunamadı: {tessPath}");
            }

            engine = new TesseractEngine(
                tessPath,
                "tur+osd",
                EngineMode.Default);

            engine.DefaultPageSegMode = PageSegMode.Auto;
            engine.SetVariable("preserve_interword_spaces", "1");
        }

        public string ReadText(Mat image)
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(OcrService));

            if (image == null || image.Empty())
                return string.Empty;

            using Mat prepared = PrepareForOcr(image);

            var sonuclar = new List<OcrSonucu>();

            using (Mat yon0 = prepared.Clone())
                sonuclar.Add(Oku(yonGorseli: yon0));

            using (Mat yon90 = new Mat())
            {
                Cv2.Rotate(
                    prepared,
                    yon90,
                    RotateFlags.Rotate90Clockwise);

                sonuclar.Add(Oku(yonGorseli: yon90));
            }

            using (Mat yon180 = new Mat())
            {
                Cv2.Rotate(
                    prepared,
                    yon180,
                    RotateFlags.Rotate180);

                sonuclar.Add(Oku(yonGorseli: yon180));
            }

            using (Mat yon270 = new Mat())
            {
                Cv2.Rotate(
                    prepared,
                    yon270,
                    RotateFlags.Rotate90Counterclockwise);

                sonuclar.Add(Oku(yonGorseli: yon270));
            }

            OcrSonucu? enIyi = sonuclar
                .Where(x => !string.IsNullOrWhiteSpace(x.Metin))
                .OrderByDescending(x => x.Skor)
                .FirstOrDefault();

            return enIyi?.Metin.Trim() ?? string.Empty;
        }

        private OcrSonucu Oku(Mat yonGorseli, bool kartMi = false)
        {
            string tempPath = Path.Combine(
                Path.GetTempPath(),
                $"belge_ocr_{Guid.NewGuid():N}.png");

            try
            {
                using (var bitmap = BitmapConverter.ToBitmap(yonGorseli))
                {
                    bitmap.Save(
                        tempPath,
                        System.Drawing.Imaging.ImageFormat.Png);
                }

                using var pix = Pix.LoadFromFile(tempPath);
                using var page = engine.Process(
                    pix,
                    PageSegMode.Auto);

                string metin = page.GetText() ?? string.Empty;

                int anlamliKarakterSayisi = metin.Count(
                    c => char.IsLetterOrDigit(c));

                int bozukKarakterSayisi = metin.Count(c =>
      !char.IsLetterOrDigit(c) &&
      !char.IsWhiteSpace(c) &&
      ".,:/()-".IndexOf(c) < 0);

                return new OcrSonucu
                {
                    Metin = metin,
                    Guven = page.GetMeanConfidence(),
                    AnlamliKarakterSayisi = anlamliKarakterSayisi,
                    AnahtarKelimePuani = AnahtarKelimePuaniHesapla(metin),
                    BozukKarakterSayisi = bozukKarakterSayisi
                };
            }
            catch
            {
                return new OcrSonucu();
            }
            finally
            {
                try
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
                catch
                {
                    // Geçici dosya silinemese bile OCR işlemini durdurma.
                }
            }
        }

        private Mat PrepareForOcr(Mat source)
        {
            Mat working = source.Clone();

            const int maxDimension = 2200;
            int largestDimension = Math.Max(working.Width, working.Height);

            if (largestDimension > maxDimension)
            {
                double scale = (double)maxDimension / largestDimension;

                Mat resized = new Mat();
                Cv2.Resize(
                    working,
                    resized,
                    new OpenCvSharp.Size(
                        Math.Max(1, (int)Math.Round(working.Width * scale)),
                        Math.Max(1, (int)Math.Round(working.Height * scale))),
                    0, 0, InterpolationFlags.Area);

                working.Dispose();
                working = resized;
            }

            Mat gray = new Mat();

            if (working.Channels() == 3)
                Cv2.CvtColor(working, gray, ColorConversionCodes.BGR2GRAY);
            else if (working.Channels() == 4)
                Cv2.CvtColor(working, gray, ColorConversionCodes.BGRA2GRAY);
            else
                working.CopyTo(gray);

            working.Dispose();

            double angle = DetectSkewAngle(gray);

            Mat deskewed;

            if (Math.Abs(angle) >= 0.35 && Math.Abs(angle) <= 20)
            {
                deskewed = RotateKeepingCanvas(gray, -angle);
                gray.Dispose();
            }
            else
            {
                deskewed = gray;
            }

            // Kimlik kartı / plastik kart benzeri belgeler için farklı yol
            if (KartBenzeriMi(deskewed))
            {
                Mat kartSonuc = PrepareCardForOcr(deskewed);
                deskewed.Dispose();
                return kartSonuc;
            }

            return PrepareDocumentForOcr(deskewed);
        }

        // CR-80 standardı: 85.60 x 53.98 mm -> en/boy oranı ~1.586
        // Kırpılmış görüntü hem yatay hem dikey olabileceğinden oranı
        // büyük kenar / küçük kenar şeklinde alıyoruz.
        private bool KartBenzeriMi(Mat gray)
        {
            double buyukKenar = Math.Max(gray.Width, gray.Height);
            double kucukKenar = Math.Min(gray.Width, gray.Height);

            if (kucukKenar <= 0)
                return false;

            double oran = buyukKenar / kucukKenar;

            return oran >= 1.35 && oran <= 1.80;
        }

        private Mat PrepareCardForOcr(Mat gray)
        {
            // Kimlik kartlarında yazı küçük olduğu için daha büyük hedef genişlik.
            Mat buyutulmus = gray;

            const int hedefGenislik = 1800;

            if (gray.Width < hedefGenislik)
            {
                double scale = (double)hedefGenislik / gray.Width;

                buyutulmus = new Mat();
                Cv2.Resize(
                    gray,
                    buyutulmus,
                    new OpenCvSharp.Size(
                        hedefGenislik,
                        Math.Max(1, (int)Math.Round(gray.Height * scale))),
                    0, 0, InterpolationFlags.Cubic);
            }

            // Yerel kontrast artırma — ama zemin desenini ezmemek için ölçülü.
            using Mat clahe1 = new Mat();
            using (var clahe = Cv2.CreateCLAHE(2.0, new OpenCvSharp.Size(16, 16)))
            {
                clahe.Apply(buyutulmus, clahe1);
            }

            if (!ReferenceEquals(buyutulmus, gray))
                buyutulmus.Dispose();

            // Hafif gürültü azaltma (bilateral, kenarları korur).
            using Mat filtered = new Mat();
            Cv2.BilateralFilter(clahe1, filtered, 5, 40, 40);

            // Hafif keskinleştirme.
            using Mat blur = new Mat();
            Cv2.GaussianBlur(filtered, blur, new OpenCvSharp.Size(0, 0), 1.0);

            Mat sharpened = new Mat();
            Cv2.AddWeighted(filtered, 1.3, blur, -0.3, 0, sharpened);

            // DİKKAT: Kağıt belgedeki gibi AdaptiveThreshold uygulanmıyor.
            // Kimlik kartı zemini (hologram/desen) sert ikili eşiklemede
            // yazının içine karışıyor. Tesseract gri tonlamalı görüntüyü
            // genelde bu tip içerikte daha iyi okuyor.
            return sharpened;
        }

        private Mat PrepareDocumentForOcr(Mat deskewed)
        {
            using Mat filtered = new Mat();
            Cv2.BilateralFilter(deskewed, filtered, 7, 50, 50);

            using Mat normalized = new Mat();
            using (var clahe = Cv2.CreateCLAHE(2.5, new OpenCvSharp.Size(8, 8)))
            {
                clahe.Apply(filtered, normalized);
            }

            using Mat blurred = new Mat();
            Cv2.GaussianBlur(normalized, blurred, new OpenCvSharp.Size(0, 0), 1.2);

            using Mat sharpened = new Mat();
            Cv2.AddWeighted(normalized, 1.4, blurred, -0.4, 0, sharpened);

            Mat binary = new Mat();
            Cv2.AdaptiveThreshold(
                sharpened, binary, 255,
                AdaptiveThresholdTypes.GaussianC,
                ThresholdTypes.Binary, 31, 11);

            if (binary.Width < 1200)
            {
                double scale = 1200.0 / binary.Width;

                Mat enlarged = new Mat();
                Cv2.Resize(
                    binary, enlarged,
                    new OpenCvSharp.Size(
                        1200,
                        Math.Max(1, (int)Math.Round(binary.Height * scale))),
                    0, 0, InterpolationFlags.Cubic);

                binary.Dispose();
                binary = enlarged;
            }

            return binary;
        }

        private double DetectSkewAngle(Mat gray)
        {
            using Mat blurred = new Mat();

            Cv2.GaussianBlur(
                gray,
                blurred,
                new OpenCvSharp.Size(5, 5),
                0);

            using Mat edges = new Mat();

            Cv2.Canny(
                blurred,
                edges,
                50,
                150);

            LineSegmentPoint[] lines = Cv2.HoughLinesP(
                edges,
                1,
                Math.PI / 180.0,
                80,
                Math.Max(40, gray.Width / 12),
                20);

            if (lines == null || lines.Length == 0)
                return 0;

            var angles = new List<double>();

            foreach (LineSegmentPoint line in lines)
            {
                double dx =
                    line.P2.X - line.P1.X;

                double dy =
                    line.P2.Y - line.P1.Y;

                if (Math.Abs(dx) < 1)
                    continue;

                double angle =
                    Math.Atan2(dy, dx) *
                    180.0 /
                    Math.PI;

                if (angle > -30 && angle < 30)
                    angles.Add(angle);
            }

            if (angles.Count == 0)
                return 0;

            angles.Sort();

            int middle = angles.Count / 2;

            return angles.Count % 2 == 0
                ? (angles[middle - 1] + angles[middle]) / 2.0
                : angles[middle];
        }

        private Mat RotateKeepingCanvas(
            Mat source,
            double angle)
        {
            Point2f center = new Point2f(
                source.Width / 2f,
                source.Height / 2f);

            using Mat rotationMatrix =
                Cv2.GetRotationMatrix2D(
                    center,
                    angle,
                    1.0);

            Mat result = new Mat();

            Cv2.WarpAffine(
                source,
                result,
                rotationMatrix,
                new OpenCvSharp.Size(
                    source.Width,
                    source.Height),
                InterpolationFlags.Linear,
                BorderTypes.Constant,
                Scalar.White);

            return result;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            engine.Dispose();
            disposed = true;
        }
    }
}
