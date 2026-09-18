using OpenCvSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace BelgeArsivlemeSistemi.OCR
{
    public class PaddleOcrService
    {
        /*
         * Python 3.11 sanal ortamındaki çalıştırıcı.
         */
        private readonly string pythonExePath =
            @"C:\Users\şeyma\PaddleOcrTest\PaddleOCR2\.venv\Scripts\python.exe";

        /*
         * Hazırladığımız PaddleOCR JSON betiği.
         */
        private readonly string pythonScriptPath =
            @"C:\Users\şeyma\PaddleOcrTest\PaddleOCR2\test_ocr.py";

        public string ReadText(Mat image)
        {
            if (image == null || image.Empty())
                return string.Empty;

            DosyalariKontrolEt();

            string geciciDosyaYolu = Path.Combine(
                Path.GetTempPath(),
                $"paddle_ocr_{Guid.NewGuid():N}.png");

            try
            {
                if (!Cv2.ImWrite(geciciDosyaYolu, image))
                {
                    throw new InvalidOperationException(
                        "OCR için geçici görsel oluşturulamadı.");
                }

                ProcessStartInfo baslatmaBilgisi =
                    new ProcessStartInfo
                    {
                        FileName = pythonExePath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    };

                /*
                 * ArgumentList kullanıldığı için boşluk ve Türkçe
                 * karakter içeren dosya yolları güvenle aktarılır.
                 */
                baslatmaBilgisi.ArgumentList.Add(
                    pythonScriptPath);

                baslatmaBilgisi.ArgumentList.Add(
                    geciciDosyaYolu);

                using Process process = new Process
                {
                    StartInfo = baslatmaBilgisi
                };

                process.Start();

                string standartCikti =
                    process.StandardOutput.ReadToEnd();

                string hataCiktisi =
                    process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    string logYolu = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "paddleocr_error.log");

                    File.WriteAllText(
                        logYolu,
                        "Çıkış kodu: " + process.ExitCode +
                        Environment.NewLine +
                        Environment.NewLine +
                        "STANDARD ERROR:" +
                        Environment.NewLine +
                        hataCiktisi +
                        Environment.NewLine +
                        Environment.NewLine +
                        "STANDARD OUTPUT:" +
                        Environment.NewLine +
                        standartCikti,
                        Encoding.UTF8);

                    throw new InvalidOperationException(
                        "PaddleOCR işlemi başarısız oldu.\n\n" +
                        HataMetniniKisalt(hataCiktisi) +
                        "\n\nAyrıntılı kayıt:\n" +
                        logYolu);
                }

                PaddleOcrResponse? sonuc =
                    JsonSonucunuOku(standartCikti);

                if (sonuc == null)
                {
                    throw new InvalidOperationException(
                        "PaddleOCR geçerli bir JSON sonucu döndürmedi.\n\n" +
                        HataMetniniKisalt(standartCikti));
                }

                if (!sonuc.Success)
                {
                    throw new InvalidOperationException(
                        "PaddleOCR hatası:\n" +
                        sonuc.Error);
                }

                return sonuc.Text?.Trim()
                    ?? string.Empty;
            }
            finally
            {
                try
                {
                    if (File.Exists(geciciDosyaYolu))
                        File.Delete(geciciDosyaYolu);
                }
                catch
                {
                    /*
                     * Geçici dosya silinemese bile
                     * uygulamanın çalışmasını bozma.
                     */
                }
            }
        }

        private void DosyalariKontrolEt()
        {
            if (!File.Exists(pythonExePath))
            {
                throw new FileNotFoundException(
                    "PaddleOCR Python çalıştırıcısı bulunamadı.",
                    pythonExePath);
            }

            if (!File.Exists(pythonScriptPath))
            {
                throw new FileNotFoundException(
                    "PaddleOCR Python betiği bulunamadı.",
                    pythonScriptPath);
            }
        }

        private PaddleOcrResponse? JsonSonucunuOku(
            string standartCikti)
        {
            if (string.IsNullOrWhiteSpace(standartCikti))
                return null;

            /*
             * Bazı kütüphaneler JSON'dan önce bilgi mesajı
             * yazdırabilir. Bu nedenle son geçerli JSON satırını
             * buluyoruz.
             */
            string? jsonSatiri = standartCikti
                .Split(
                    new[] { '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(satir => satir.Trim())
                .LastOrDefault(satir =>
                    satir.StartsWith("{") &&
                    satir.EndsWith("}"));

            if (string.IsNullOrWhiteSpace(jsonSatiri))
                return null;

            return JsonSerializer.Deserialize<PaddleOcrResponse>(
                jsonSatiri,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        private string HataMetniniKisalt(string metin)
        {
            if (string.IsNullOrWhiteSpace(metin))
                return "Ayrıntılı hata mesajı bulunmuyor.";

            const int maksimumUzunluk = 2000;

            metin = metin.Trim();

            if (metin.Length <= maksimumUzunluk)
                return metin;

            return metin.Substring(
                metin.Length - maksimumUzunluk);
        }

        private class PaddleOcrResponse
        {
            public bool Success { get; set; }

            public string Error { get; set; } =
                string.Empty;

            public string Text { get; set; } =
                string.Empty;

            public PaddleOcrLine[] Lines { get; set; } =
                Array.Empty<PaddleOcrLine>();
        }

        private class PaddleOcrLine
        {
            public string Text { get; set; } =
                string.Empty;

            public double Confidence { get; set; }
        }
    }
}