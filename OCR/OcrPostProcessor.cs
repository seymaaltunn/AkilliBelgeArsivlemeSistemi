using System.Text.RegularExpressions;
using System.Linq;

namespace BelgeArsivlemeSistemi.OCR
{
    public class OcrPostProcessor
    {
        public string Temizle(string metin)
        {
            if (string.IsNullOrWhiteSpace(metin))
                return "";

            metin = metin.Replace("\r\n", "\n");

            metin = metin.Replace("*", "");
            metin = metin.Replace("|", "I");
            metin = metin.Replace("¦", "I");
            metin = metin.Replace("`", "");
            metin = metin.Replace("'", "");

            metin = metin.Replace("Â", "A");
            metin = metin.Replace("â", "a");
            metin = metin.Replace("Ð", "Ğ");

            metin = metin.Replace("YAAPI", "YAPI");
            metin = metin.Replace("YAAPl", "YAPI");
            metin = metin.Replace("RUHSATl", "RUHSATI");
            metin = metin.Replace("BELEDIYE", "BELEDİYE");
            metin = metin.Replace("BASKANLIGI", "BAŞKANLIĞI");
            metin = metin.Replace("Numaraar", "Numarası");

            metin = Regex.Replace(metin, @"[ ]{2,}", " ");
            metin = Regex.Replace(metin, @"(\n\s*){3,}", "\n\n");

            return metin.Trim();
        }
        // OcrPostProcessor.cs içine eklenecek
        public bool AnlamsizMi(string metin)
        {
            if (string.IsNullOrWhiteSpace(metin))
                return true;

            string harfler = new string(
                metin
                    .Where(char.IsLetter)
                    .Select(char.ToUpperInvariant)
                    .ToArray());

            if (harfler.Length < 15)
                return true;

            int farkliKarakterSayisi =
                harfler.Distinct().Count();

            double cesitlilikOrani =
                (double)farkliKarakterSayisi /
                harfler.Length;

            if (cesitlilikOrani < 0.12)
                return true;

            // VVVVV, MMMMM veya benzeri tekrarları yakalar.
            if (Regex.IsMatch(
                harfler,
                @"(.)\1{3,}",
                RegexOptions.IgnoreCase))
            {
                return true;
            }

            string[] kelimeler = Regex
                .Split(metin, @"\s+")
                .Where(k => k.Count(char.IsLetter) >= 2)
                .ToArray();

            // Uzun OCR çıktısında neredeyse hiç gerçek kelime oluşmamışsa.
            if (harfler.Length >= 30 && kelimeler.Length < 2)
                return true;

            return false;
        }
    }
}