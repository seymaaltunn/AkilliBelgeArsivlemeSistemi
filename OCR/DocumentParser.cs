using BelgeArsivlemeSistemi.Models;
using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BelgeArsivlemeSistemi.OCR
{
    public class DocumentParser
    {
        private class BelgeSozlukKaydi
        {
            public string Anahtar { get; set; } = "";
            public string BelgeAdi { get; set; } = "";
            public string BelgeTuru { get; set; } = "";
        }

        private readonly List<BelgeSozlukKaydi> belgeSozlugu =
            new List<BelgeSozlukKaydi>
            {
                new BelgeSozlukKaydi
                {
                    Anahtar = "YAPI RUHSATI",
                    BelgeAdi = "Yapı Ruhsatı",
                    BelgeTuru = "Ruhsat"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "BUILDING LICENCE",
                    BelgeAdi = "Yapı Ruhsatı",
                    BelgeTuru = "Ruhsat"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "BUILDING LICENSE",
                    BelgeAdi = "Yapı Ruhsatı",
                    BelgeTuru = "Ruhsat"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "YAPI KULLANMA İZİN BELGESİ",
                    BelgeAdi = "Yapı Kullanma İzin Belgesi",
                    BelgeTuru = "Yapı Kullanma İzin Belgesi"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "İSKAN RUHSATI",
                    BelgeAdi = "Yapı Kullanma İzin Belgesi",
                    BelgeTuru = "Yapı Kullanma İzin Belgesi"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "YERLEŞİM YERİ",
                    BelgeAdi = "Yerleşim Yeri ve Diğer Adres Belgesi",
                    BelgeTuru = "Kimlik / Adres Belgesi"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "ADRES BELGESİ",
                    BelgeAdi = "Yerleşim Yeri ve Diğer Adres Belgesi",
                    BelgeTuru = "Kimlik / Adres Belgesi"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "KİMLİK BİLGİLERİ",
                    BelgeAdi = "Kimlik Bilgileri",
                    BelgeTuru = "Kimlik / Adres Belgesi"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "FATURA",
                    BelgeAdi = "Fatura",
                    BelgeTuru = "Fatura / Mali Evrak"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "E-ARŞİV FATURA",
                    BelgeAdi = "E-Arşiv Fatura",
                    BelgeTuru = "Fatura / Mali Evrak"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "E-FATURA",
                    BelgeAdi = "E-Fatura",
                    BelgeTuru = "Fatura / Mali Evrak"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "DİLEKÇE",
                    BelgeAdi = "Dilekçe",
                    BelgeTuru = "Dilekçe / Başvuru"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "İHALE",
                    BelgeAdi = "İhale Dosyası",
                    BelgeTuru = "İhale Dosyası"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "SÖZLEŞME",
                    BelgeAdi = "Sözleşme",
                    BelgeTuru = "Sözleşme"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "MECLİS KARARI",
                    BelgeAdi = "Meclis Kararı",
                    BelgeTuru = "Meclis / Encümen Kararı"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "ENCÜMEN KARARI",
                    BelgeAdi = "Encümen Kararı",
                    BelgeTuru = "Meclis / Encümen Kararı"
                },
                new BelgeSozlukKaydi
                {
                    Anahtar = "RUHSAT",
                    BelgeAdi = "Ruhsat",
                    BelgeTuru = "Ruhsat"
                }
            };

        public DocumentInfo Parse(string metin)
        {
            metin = metin ?? "";

            BelgeSozlukKaydi? kayit = EnYakinBelgeKaydi(metin);

            string belgeAdi =
                kayit?.BelgeAdi ??
                TahminBelgeAdi(metin);

            string belgeTuru =
                kayit?.BelgeTuru ??
                TahminBelgeTuru(metin);

            string kurum =
                KurumBul(metin, belgeTuru);

            string kisiKurum =
                BulKisiKurum(metin, belgeTuru, kurum);

            return new DocumentInfo
            {
                BelgeAdi = belgeAdi,
                BelgeTuru = belgeTuru,
                KimlikNo = BulKimlikNo(metin),
                KisiKurum = kisiKurum,
                Kurum = kurum,
                Tarih = BulTarih(metin),
                EvrakNo = BulEvrakNo(metin),
                Aciklama = metin.Trim()
            };
        }

        private static int LevenshteinMesafesi(string birinci, string ikinci)
        {
            birinci ??= string.Empty;
            ikinci ??= string.Empty;

            int[,] tablo = new int[birinci.Length + 1, ikinci.Length + 1];

            for (int i = 0; i <= birinci.Length; i++)
                tablo[i, 0] = i;

            for (int j = 0; j <= ikinci.Length; j++)
                tablo[0, j] = j;

            for (int i = 1; i <= birinci.Length; i++)
            {
                for (int j = 1; j <= ikinci.Length; j++)
                {
                    int maliyet =
                        birinci[i - 1] == ikinci[j - 1] ? 0 : 1;

                    tablo[i, j] = Math.Min(
                        Math.Min(
                            tablo[i - 1, j] + 1,
                            tablo[i, j - 1] + 1),
                        tablo[i - 1, j - 1] + maliyet);
                }
            }

            return tablo[birinci.Length, ikinci.Length];
        }

        private BelgeSozlukKaydi? EnYakinBelgeKaydi(
            string metin,
            int esik = 80)
        {
            if (string.IsNullOrWhiteSpace(metin))
                return null;

            BelgeSozlukKaydi? enIyiKayit = null;
            int enYuksekSkor = 0;

            foreach (BelgeSozlukKaydi kayit in belgeSozlugu)
            {
                int skor = Fuzz.PartialRatio(
                    metin.ToUpperInvariant(),
                    kayit.Anahtar.ToUpperInvariant());

                if (skor > enYuksekSkor)
                {
                    enYuksekSkor = skor;
                    enIyiKayit = kayit;
                }
            }

            return enYuksekSkor >= esik
                ? enIyiKayit
                : null;
        }

        private string TahminBelgeAdi(string metin)
        {
            if (BenziyorMu(metin, "YAPI KULLANMA İZİN BELGESİ") ||
                BenziyorMu(metin, "İSKAN RUHSATI"))
            {
                return "Yapı Kullanma İzin Belgesi";
            }

            if (BenziyorMu(metin, "YAPI RUHSATI") ||
                BenziyorMu(metin, "BUILDING LICENCE") ||
                BenziyorMu(metin, "BUILDING LICENSE"))
            {
                return "Yapı Ruhsatı";
            }

            if (BenziyorMu(metin, "E-ARŞİV FATURA"))
                return "E-Arşiv Fatura";

            if (BenziyorMu(metin, "E-FATURA"))
                return "E-Fatura";

            if (BenziyorMu(metin, "FATURA"))
                return "Fatura";

            if (BenziyorMu(metin, "YERLEŞİM YERİ") ||
                BenziyorMu(metin, "ADRES BELGESİ"))
            {
                return "Yerleşim Yeri Belgesi";
            }

            if (BenziyorMu(metin, "DİLEKÇE"))
                return "Dilekçe";

            if (BenziyorMu(metin, "SÖZLEŞME"))
                return "Sözleşme";

            string[] satirlar = SatirlariAl(metin);

            foreach (string satir in satirlar.Take(15))
            {
                string temizSatir = BaslikSatiriniTemizle(satir);

                if (BaslikOlabilirMi(temizSatir))
                {
                    return temizSatir.Length > 80
                        ? temizSatir.Substring(0, 80)
                        : temizSatir;
                }
            }

            return "Adsız Belge";
        }

        private string BaslikSatiriniTemizle(string satir)
        {
            if (string.IsNullOrWhiteSpace(satir))
                return string.Empty;

            string temiz = Regex.Replace(
                satir,
                @"[^\p{L}\p{N}\s\-/]",
                " ");

            temiz = Regex.Replace(
                temiz,
                @"\s+",
                " ").Trim();

            return temiz;
        }

        private string TahminBelgeTuru(string metin)
        {
            if (string.IsNullOrWhiteSpace(metin))
                return "Diğer";

            string normalizeMetin = MetniNormalizeEt(metin);

            
            if (normalizeMetin.Contains("YAPI KULLANMA") ||
                normalizeMetin.Contains("ISKAN RUHSAT") ||
                BenzerKelimeVarMi(normalizeMetin, "ISKAN", 2))
            {
                return "Yapı Kullanma İzin Belgesi";
            }

            if (normalizeMetin.Contains("YAPI RUHSATI") ||
                normalizeMetin.Contains("BUILDING LICENCE") ||
                normalizeMetin.Contains("BUILDING LICENSE") ||
                BenzerKelimeVarMi(normalizeMetin, "RUHSAT", 2))
            {
                return "Ruhsat";
            }

            if (normalizeMetin.Contains("YERLESIM YERI") ||
                normalizeMetin.Contains("ADRES BELGESI") ||
                normalizeMetin.Contains("KIMLIK NO") ||
                normalizeMetin.Contains("T C KIMLIK") ||
                BenzerKelimeVarMi(normalizeMetin, "KIMLIK", 2) ||
                BenzerKelimeVarMi(normalizeMetin, "IDENTITY", 2))
            {
                return "Kimlik / Adres Belgesi";
            }

            if (BenzerKelimeVarMi(normalizeMetin, "DILEKCE", 2) ||
                normalizeMetin.Contains("ARZ EDERIM") ||
                BenzerKelimeVarMi(normalizeMetin, "BASVURU", 2))
            {
                return "Dilekçe / Başvuru";
            }

            if (BenzerKelimeVarMi(normalizeMetin, "FATURA", 2) ||
                normalizeMetin.Contains("E ARSIV") ||
                normalizeMetin.Contains("E FATURA") ||
                normalizeMetin.Contains("ODENECEK TUTAR") ||
                normalizeMetin.Contains("KDV"))
            {
                return "Fatura / Mali Evrak";
            }

            if (BenzerKelimeVarMi(normalizeMetin, "IHALE", 2) ||
                BenzerKelimeVarMi(normalizeMetin, "SARTNAME", 2) ||
                BenzerKelimeVarMi(normalizeMetin, "YUKLENICI", 2))
            {
                return "İhale Dosyası";
            }

            if (BenzerKelimeVarMi(normalizeMetin, "IMAR", 2) ||
                normalizeMetin.Contains("ADA") ||
                normalizeMetin.Contains("PARSEL"))
            {
                return "İmar / Şehircilik Belgesi";
            }

            if (BenzerKelimeVarMi(normalizeMetin, "MECLIS", 2) ||
                BenzerKelimeVarMi(normalizeMetin, "ENCUMEN", 2) ||
                BenzerKelimeVarMi(normalizeMetin, "KARAR", 2))
            {
                return "Meclis / Encümen Kararı";
            }

            if (BenzerKelimeVarMi(normalizeMetin, "SOZLESME", 2))
                return "Sözleşme";

            return "Diğer";
        }

        private static string MetniNormalizeEt(string metin)
        {
            if (string.IsNullOrWhiteSpace(metin))
                return string.Empty;

            string sonuc = metin.ToUpperInvariant();

            sonuc = sonuc
                .Replace('1', 'I')
                .Replace('0', 'O')
                .Replace('|', 'I')
                .Replace('İ', 'I')
                .Replace('Ş', 'S')
                .Replace('Ğ', 'G')
                .Replace('Ü', 'U')
                .Replace('Ö', 'O')
                .Replace('Ç', 'C');

            sonuc = Regex.Replace(sonuc, @"[^A-Z0-9\s]", " ");
            sonuc = Regex.Replace(sonuc, @"\s+", " ").Trim();

            return sonuc;
        }

        private static bool BenzerKelimeVarMi(
    string metin,
    string arananKelime,
    int maksimumHata = 2)
        {
            string normalizeMetin = MetniNormalizeEt(metin);
            string normalizeAranan = MetniNormalizeEt(arananKelime);

            if (normalizeMetin.Contains(normalizeAranan))
                return true;

            string[] kelimeler = normalizeMetin.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string kelime in kelimeler)
            {
                if (Math.Abs(kelime.Length - normalizeAranan.Length) > maksimumHata)
                    continue;

                if (LevenshteinMesafesi(kelime, normalizeAranan) <= maksimumHata)
                    return true;
            }

            return false;
        }

        private string BulKisiKurum(
            string metin,
            string belgeTuru,
            string kurum)
        {
            string adSoyad = BulAdSoyad(metin);

            if (!string.IsNullOrWhiteSpace(adSoyad))
                return adSoyad;

            if (!string.IsNullOrWhiteSpace(kurum))
                return kurum;

            return KurumBul(metin, belgeTuru);
        }

        private string BulAdSoyad(string metin)
        {
            Match adSoyadEtiketli = Regex.Match(
                metin,
                @"(?:Ad[ıi]\s*Soyad[ıi]|Ad\s*Soyad)\s*:?\s*" +
                @"([A-ZÇĞİÖŞÜ][A-ZÇĞİÖŞÜa-zçğıöşü]+(?:\s+" +
                @"[A-ZÇĞİÖŞÜ][A-ZÇĞİÖŞÜa-zçğıöşü]+){1,3})",
                RegexOptions.IgnoreCase);

            if (adSoyadEtiketli.Success)
                return adSoyadEtiketli.Groups[1].Value.Trim();

            Match ad = Regex.Match(
                metin,
                @"\bAd[ıi]\s*:?\s*([A-ZÇĞİÖŞÜa-zçğıöşü]+)",
                RegexOptions.IgnoreCase);

            Match soyad = Regex.Match(
                metin,
                @"\bSoyad[ıi]\s*:?\s*([A-ZÇĞİÖŞÜa-zçğıöşü]+)",
                RegexOptions.IgnoreCase);

            if (ad.Success && soyad.Success)
            {
                return ad.Groups[1].Value.Trim() +
                       " " +
                       soyad.Groups[1].Value.Trim();
            }

            return "";
        }

        private string KurumBul(
            string metin,
            string belgeTuru)
        {
            if (belgeTuru.Contains(
                "Fatura",
                StringComparison.OrdinalIgnoreCase))
            {
                string faturaKurumu =
                    FaturadanKurumBul(metin);

                if (!string.IsNullOrWhiteSpace(faturaKurumu))
                    return faturaKurumu;
            }

            if (belgeTuru.Contains(
                    "Ruhsat",
                    StringComparison.OrdinalIgnoreCase) ||
                belgeTuru.Contains(
                    "Yapı Kullanma",
                    StringComparison.OrdinalIgnoreCase))
            {
                string ruhsatKurumu =
                    RuhsattanKurumBul(metin);

                if (!string.IsNullOrWhiteSpace(ruhsatKurumu))
                    return ruhsatKurumu;
            }

            return GenelKurumBul(metin);
        }

        private string RuhsattanKurumBul(string metin)
        {
            string[] satirlar = SatirlariAl(metin);

            string[] anahtarlar =
            {
                "BELEDİYE BAŞKANLIĞI",
                "BELEDİYESİ",
                "BELEDIYESI",
                "İMAR VE ŞEHİRCİLİK",
                "İMAR MÜDÜRLÜĞÜ",
                "YAPI KONTROL MÜDÜRLÜĞÜ",
                "FEN İŞLERİ MÜDÜRLÜĞÜ",
                "BAŞKANLIĞI",
                "MÜDÜRLÜĞÜ"
            };

            foreach (string satir in satirlar.Take(30))
            {
                if (anahtarlar.Any(anahtar =>
                    satir.Contains(
                        anahtar,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    return KurumSatiriniTemizle(satir);
                }
            }

            return "";
        }

        private string FaturadanKurumBul(string metin)
        {
            string[] satirlar = SatirlariAl(metin);

            string[] kurumAnahtarlari =
            {
                "LTD",
                "LİMİTED",
                "LIMITED",
                "ŞTİ",
                "ŞİRKET",
                "A.Ş",
                "ANONİM",
                "TİCARET",
                "SANAYİ",
                "MOBILE",
                "MOBİLE",
                "TELEKOM",
                "ELEKTRİK",
                "DOĞALGAZ",
                "SU VE KANALİZASYON",
                "BELEDİYE"
            };

            foreach (string satir in satirlar.Take(20))
            {
                if (FaturaKurumSatiriMi(
                    satir,
                    kurumAnahtarlari))
                {
                    return KurumSatiriniTemizle(satir);
                }
            }

            string[] etiketler =
            {
                "SATICI",
                "FİRMA",
                "TİCARET ÜNVANI",
                "ÜNVAN",
                "UNVAN"
            };

            for (int i = 0; i < satirlar.Length; i++)
            {
                if (!etiketler.Any(etiket =>
                    satirlar[i].Contains(
                        etiket,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                string ayniSatir = Regex.Replace(
                    satirlar[i],
                    @"^(SATICI|FİRMA|TİCARET\s+ÜNVANI|ÜNVAN|UNVAN)\s*:?\s*",
                    "",
                    RegexOptions.IgnoreCase).Trim();

                if (FaturaKurumSatiriMi(
                    ayniSatir,
                    kurumAnahtarlari,
                    anahtarZorunlu: false))
                {
                    return KurumSatiriniTemizle(ayniSatir);
                }

                if (i + 1 < satirlar.Length &&
                    FaturaKurumSatiriMi(
                        satirlar[i + 1],
                        kurumAnahtarlari,
                        anahtarZorunlu: false))
                {
                    return KurumSatiriniTemizle(satirlar[i + 1]);
                }
            }

            foreach (string satir in satirlar.Take(8))
            {
                if (FaturaKurumSatiriMi(
                    satir,
                    kurumAnahtarlari,
                    anahtarZorunlu: false))
                {
                    return KurumSatiriniTemizle(satir);
                }
            }

            return "";
        }

        private bool FaturaKurumSatiriMi(
            string satir,
            string[] kurumAnahtarlari,
            bool anahtarZorunlu = true)
        {
            if (string.IsNullOrWhiteSpace(satir))
                return false;

            string temiz = satir.Trim();

            if (temiz.Length < 3 || temiz.Length > 100)
                return false;

            string lower = temiz.ToLowerInvariant();

            string[] yasakliIfadeler =
            {
                "fatura",
                "fatura no",
                "fatura tarihi",
                "vergi no",
                "vergi dairesi",
                "mersis",
                "tckn",
                "vkn",
                "telefon",
                "tel:",
                "e-posta",
                "email",
                "adres",
                "sayın",
                "mal hizmet",
                "miktar",
                "birim fiyat",
                "kdv",
                "toplam",
                "ödenecek"
            };

            if (yasakliIfadeler.Any(lower.Contains))
                return false;

            int harfSayisi =
                temiz.Count(char.IsLetter);

            if (harfSayisi < 3)
                return false;

            if (anahtarZorunlu)
            {
                return kurumAnahtarlari.Any(anahtar =>
                    temiz.Contains(
                        anahtar,
                        StringComparison.OrdinalIgnoreCase));
            }

            bool sadeceBuyukHarfliBaslik =
                temiz.Count(char.IsLetter) >= 3 &&
                temiz.Where(char.IsLetter)
                    .All(char.IsUpper);

            return sadeceBuyukHarfliBaslik ||
                   kurumAnahtarlari.Any(anahtar =>
                       temiz.Contains(
                           anahtar,
                           StringComparison.OrdinalIgnoreCase));
        }

        private string GenelKurumBul(string metin)
        {
            string[] satirlar = SatirlariAl(metin);

            string[] anahtarlar =
            {
                "BELEDİYE BAŞKANLIĞI",
                "BELEDİYESİ",
                "BELEDIYESI",
                "BAKANLIĞI",
                "MÜDÜRLÜĞÜ",
                "MUDURLUGU",
                "VALİLİĞİ",
                "KAYMAKAMLIĞI",
                "ÜNİVERSİTESİ",
                "GENEL MÜDÜRLÜĞÜ",
                "LTD",
                "ŞTİ",
                "A.Ş",
                "ANONİM",
                "TİCARET",
                "SANAYİ"
            };

            foreach (string satir in satirlar.Take(30))
            {
                if (anahtarlar.Any(anahtar =>
                    satir.Contains(
                        anahtar,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    return KurumSatiriniTemizle(satir);
                }
            }

            string lower = metin.ToLowerInvariant();

            if (lower.Contains("içişleri bakanlığı"))
                return "T.C. İçişleri Bakanlığı";

            if (lower.Contains("nüfus ve vatandaşlık"))
            {
                return "Nüfus ve Vatandaşlık İşleri Genel Müdürlüğü";
            }

            return "";
        }

        private string BulKimlikNo(string metin)
        {
            Match match = Regex.Match(
                metin,
                @"(?<!\d)\d{11}(?!\d)");

            return match.Success
                ? match.Value
                : "";
        }

        private string BulTarih(string metin)
        {
            MatchCollection tarihler = Regex.Matches(
                metin,
                @"(?<!\d)(0?[1-9]|[12]\d|3[01])[./-]" +
                @"(0?[1-9]|1[0-2])[./-]" +
                @"((?:19|20)\d{2})(?!\d)");

            if (tarihler.Count == 0)
                return "";

            return tarihler[0].Value;
        }

        private string BulEvrakNo(string metin)
        {
            Match match = Regex.Match(
                metin,
                @"(?:Evrak\s*No|Belge\s*No|Ruhsat\s*No|" +
                @"Fatura\s*No|Sayı|No)\s*:?\s*" +
                @"([A-Za-z0-9ÇĞİÖŞÜçğıöşü\/\.\-_]+)",
                RegexOptions.IgnoreCase);

            return match.Success
                ? match.Groups[1].Value.Trim()
                : "";
        }

        private bool BenziyorMu(
            string metin,
            string aranacak,
            int esik = 85)
        {
            if (string.IsNullOrWhiteSpace(metin))
                return false;

            return Fuzz.PartialRatio(
                metin.ToUpperInvariant(),
                aranacak.ToUpperInvariant()) >= esik;
        }

        private string[] SatirlariAl(string metin)
        {
            return (metin ?? "")
                .Split(
                    new[] { '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Regex.Replace(s.Trim(), @"\s+", " "))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
        }

        private bool BaslikOlabilirMi(string satir)
        {
            if (string.IsNullOrWhiteSpace(satir))
                return false;

            satir = satir.Trim();

            // "M", "I", "X" gibi tek karakterli OCR çöplerini engelle.
            if (satir.Length < 5)
                return false;

            int harfSayisi = satir.Count(char.IsLetter);
            int rakamSayisi = satir.Count(char.IsDigit);

            // Başlıkta en az dört harf bulunmalı.
            if (harfSayisi < 4)
                return false;

            // Satırın büyük bölümü harflerden oluşmalı.
            double harfOrani =
                (double)harfSayisi /
                Math.Max(1, satir.Length);

            if (harfOrani < 0.45)
                return false;

            // Yalnızca rakam veya işaretlerden oluşan satırları engelle.
            if (harfSayisi == 0 && rakamSayisi > 0)
                return false;

            string lower = satir.ToLowerInvariant();

            string[] anlamsizBasliklar =
            {
        "t.c.",
        "t.c",
        "tarih",
        "sayı",
        "no",
        "adres",
        "telefon",
        "e-posta",
        "vergi",
        "sayfa",
        "ad",
        "soyad",
        "adı",
        "soyadı"
    };

            if (anlamsizBasliklar.Any(x =>
                lower == x ||
                lower.StartsWith(x + ":")))
            {
                return false;
            }

            return true;
        }

        private string KurumSatiriniTemizle(string satir)
        {
            if (string.IsNullOrWhiteSpace(satir))
                return "";

            string temiz = Regex.Replace(
                satir,
                @"[^\p{L}\p{N}\s.\-/]",
                " ");

            temiz = Regex.Replace(temiz, @"\s+", " ").Trim();

            temiz = temiz.Trim(
                ' ',
                '-',
                ':',
                ';',
                ',',
                '.',
                '|');

            return temiz.Length > 120
                ? temiz.Substring(0, 120)
                : temiz;
        }
    }
}