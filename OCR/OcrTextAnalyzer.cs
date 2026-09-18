using System;

namespace BelgeArsivlemeSistemi.OCR
{
    public class OcrTextAnalyzer
    {
        public string TahminBelgeAdi(string metin)
        {
            if (string.IsNullOrWhiteSpace(metin))
                return "Adsız Belge";

            string[] satirlar = metin.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string satir in satirlar)
            {
                string temiz = satir.Trim();

                if (temiz.Length > 8)
                    return temiz.Length > 80 ? temiz.Substring(0, 80) : temiz;
            }

            return "Adsız Belge";
        }

        public string TahminBelgeTuru(string metin)
        {
            string lower = metin.ToLower();

            if (lower.Contains("fatura"))
                return "Fatura";

            if (lower.Contains("dilekçe") || lower.Contains("arz ederim"))
                return "Dilekçe";

            if (lower.Contains("sözleşme"))
                return "Sözleşme";

            if (lower.Contains("kimlik") || lower.Contains("t.c."))
                return "Kimlik Fotokopisi";

            if (lower.Contains("ruhsat"))
                return "Ruhsat";

            return "Diğer";
        }
    }
}