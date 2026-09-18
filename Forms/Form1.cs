using BelgeArsivlemeSistemi.AI;
using BelgeArsivlemeSistemi.Database;
using BelgeArsivlemeSistemi.ImageProcessing;
using BelgeArsivlemeSistemi.Models;
using BelgeArsivlemeSistemi.OCR;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BelgeArsivlemeSistemi
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            yoloService = new YoloService();

            DatabaseInitializer.Initialize();
        }


        private Mat? orijinalResim;
        private Mat? aktifResim;
        private Mat? oncekiResim;

        private readonly YoloService yoloService;
        private readonly OcrService ocrService = new OcrService();
        private readonly OcrPostProcessor ocrCleaner = new OcrPostProcessor();
        private readonly DocumentParser documentParser = new DocumentParser();
        private readonly DocumentVisibilityService visibilityService =
            new DocumentVisibilityService();

        // Manuel görüntü iþlemleri tasarýmda kalýrsa bu iki servis kullanýlmaya devam eder.
        private readonly ImageEditor imageEditor = new ImageEditor();
        private readonly DocumentEnhancer enhancer = new DocumentEnhancer();

        private readonly List<Mat> bulunanBelgeler = new List<Mat>();

        private int aktifBelgeIndex;
        private bool aktifBelgeKimlikVeyaAdres;

        private readonly DatabaseService databaseService =
    new DatabaseService();

        private double CakismaOrani(Rect a, Rect b)
        {
            int sol = Math.Max(a.Left, b.Left);
            int ust = Math.Max(a.Top, b.Top);
            int sag = Math.Min(a.Right, b.Right);
            int alt = Math.Min(a.Bottom, b.Bottom);

            int genislik = Math.Max(0, sag - sol);
            int yukseklik = Math.Max(0, alt - ust);

            double kesisimAlani = genislik * yukseklik;

            double kucukKutuAlani = Math.Min(
                a.Width * a.Height,
                b.Width * b.Height);

            return kucukKutuAlani <= 0
                ? 0
                : kesisimAlani / kucukKutuAlani;
        }

        private double BelgePuani(
     Rect rect,
     string metin,
     int resimAlani)
        {
            int karakterSayisi = string.IsNullOrWhiteSpace(metin)
                ? 0
                : metin.Count(char.IsLetterOrDigit);

            int satirSayisi = string.IsNullOrWhiteSpace(metin)
                ? 0
                : metin.Split(
                    new[] { '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries).Length;

            double alanOrani =
                (double)(rect.Width * rect.Height) / resimAlani;

            double puan = 0;

            puan += Math.Min(karakterSayisi / 10.0, 50);
            puan += Math.Min(satirSayisi * 2.0, 20);
            puan += Math.Min(alanOrani * 100, 30);

            return puan;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            cmbBelgeTuru.Items.Clear();
            cmbBelgeTuru.Items.AddRange(new object[]
            {
                "Kurumsal / Ýdari Belge",
                "Meclis / Encümen Kararý",
                "Gelen / Giden Evrak",
                "Genelge / Yönetmelik",
                "Ýmar / Þehircilik Belgesi",
                "Ýmar Planý / Pafta",
                "Ruhsat",
                "Mimari Proje",
                "Mali / Satýn Alma Belgesi",
                "Ýhale Dosyasý",
                "Fatura / Mali Evrak",
                "Vatandaþ Hizmeti Belgesi",
                "Dilekçe / Baþvuru",
                "Sosyal Yardým Belgesi",
                "Evlendirme / Emlak Dosyasý",
                "Kimlik / Adres Belgesi",
                "Diðer"
            });

            cmbBelgeTuru.SelectedIndex = 0;
            lblBelgeSirasi.Text = "Belge 0 / 0";
        }

        #region Temel görüntü iþlemleri

        private void ResmiGoster(Mat resim)
        {
            Mat yeniAktifResim = resim.Clone();

            aktifResim?.Dispose();
            aktifResim = yeniAktifResim;

            pictureBoxBelge.Image?.Dispose();
            pictureBoxBelge.Image = BitmapConverter.ToBitmap(aktifResim);
            pictureBoxBelge.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void BelgeleriTemizle()
        {
            foreach (Mat belge in bulunanBelgeler)
            {
                belge.Dispose();
            }

            bulunanBelgeler.Clear();
            aktifBelgeIndex = 0;
            lblBelgeSirasi.Text = "Belge 0 / 0";
        }

        private bool AktifResimVarMi()
        {
            if (aktifResim != null && !aktifResim.Empty())
                return true;

            MessageBox.Show("Önce resim seçiniz.");
            return false;
        }

        private void GeriAlIcinSakla()
        {
            oncekiResim?.Dispose();
            oncekiResim = aktifResim?.Clone();
        }

        private Rect GenisletilmisRect(
            Rect rect,
            int maxWidth,
            int maxHeight,
            int pay)
        {
            int x = Math.Max(0, rect.X - pay);
            int y = Math.Max(0, rect.Y - pay);
            int sag = Math.Min(maxWidth, rect.Right + pay);
            int alt = Math.Min(maxHeight, rect.Bottom + pay);

            return new Rect(x, y, sag - x, alt - y);
        }

        #endregion

        #region Belge seçme, algýlama ve gezinme

        private void btnResimSec_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dosyaSec = new OpenFileDialog
            {
                Title = "Belge Seç",
                Filter = "Resimler|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dosyaSec.ShowDialog() != DialogResult.OK)
                return;

            Mat yeniResim = Cv2.ImRead(dosyaSec.FileName);

            if (yeniResim.Empty())
            {
                yeniResim.Dispose();
                MessageBox.Show("Seçilen görsel açýlamadý.");
                return;
            }

            BelgeleriTemizle();

            orijinalResim?.Dispose();
            orijinalResim = yeniResim;

            ResmiGoster(orijinalResim);
            FormAlanlariniTemizle();
        }

        private void btnBelgeBul_Click(object sender, EventArgs e)
        {
            if (orijinalResim == null || orijinalResim.Empty())
            {
                MessageBox.Show("Önce resim seçiniz.");
                return;
            }

            BelgeleriTemizle();

            List<Rect> kutular =
                yoloService.BelgeleriTespitEt(orijinalResim, 0.30f);

            if (kutular.Count == 0)
            {
                MessageBox.Show("Belge bulunamadý.");
                return;
            }

            int resimAlani = orijinalResim.Width * orijinalResim.Height;

            var adaylar =
                new List<(Rect Kutu, Mat Resim, string Metin, double Puan)>();

            foreach (Rect kutu in kutular)
            {
                Rect rect = GenisletilmisRect(
                    kutu,
                    orijinalResim.Width,
                    orijinalResim.Height,
                    15);

                Mat kirpilmis =
                    new Mat(orijinalResim, rect).Clone();

                string metin =
                    ocrService.ReadText(kirpilmis);

                metin = ocrCleaner.Temizle(metin);

                double puan = BelgePuani(
                    rect,
                    metin,
                    resimAlani);

                adaylar.Add((
                    rect,
                    kirpilmis,
                    metin,
                    puan));
            }

            var secilenler =
                new List<(Rect Kutu, Mat Resim, string Metin, double Puan)>();

            int okunamayanBelgeSayisi = 0;

            foreach (var aday in adaylar.OrderByDescending(x => x.Puan))
            {
                int karakterSayisi =
                    aday.Metin.Count(char.IsLetterOrDigit);

                if (karakterSayisi < 20)
                {
                    okunamayanBelgeSayisi++;
                    aday.Resim.Dispose();
                    continue;
                }

                bool ayniBelgeGrubu = secilenler.Any(secilen =>
                    CakismaOrani(aday.Kutu, secilen.Kutu) > 0.20);

                if (ayniBelgeGrubu)
                {
                    okunamayanBelgeSayisi++;
                    aday.Resim.Dispose();
                    continue;
                }

                bool tamGorunuyor =
                    visibilityService.TamGorunuyorMu(
                        aday.Kutu,
                        aday.Resim,
                        aday.Metin,
                        orijinalResim.Width,
                        orijinalResim.Height);

                if (!tamGorunuyor)
                {
                    okunamayanBelgeSayisi++;
                    aday.Resim.Dispose();
                    continue;
                }

                secilenler.Add(aday);
            }

            foreach (var belge in secilenler)
            {
                bulunanBelgeler.Add(belge.Resim);
            }

            if (bulunanBelgeler.Count == 0)
            {
                lblBelgeSirasi.Text = "Belge 0 / 0";

                MessageBox.Show(
                    $"{kutular.Count} belge adayý algýlandý.\n" +
                    "Okunabilir belge bulunamadý.");

                return;
            }

            aktifBelgeIndex = 0;
            AktifBelgeyiGosterVeOku();

            MessageBox.Show(
                $"{kutular.Count} belge adayý algýlandý.\n" +
                $"{bulunanBelgeler.Count} belge okunabilir.\n" +
                $"{okunamayanBelgeSayisi} aday elendi.");
        }

        private double OrtulmeOrani(Rect onBelge, Rect digerBelge)
        {
            int sol = Math.Max(onBelge.Left, digerBelge.Left);
            int ust = Math.Max(onBelge.Top, digerBelge.Top);
            int sag = Math.Min(onBelge.Right, digerBelge.Right);
            int alt = Math.Min(onBelge.Bottom, digerBelge.Bottom);

            int genislik = Math.Max(0, sag - sol);
            int yukseklik = Math.Max(0, alt - ust);

            double kesisimAlani = genislik * yukseklik;
            double digerBelgeAlani = digerBelge.Width * digerBelge.Height;

            return digerBelgeAlani <= 0
                ? 0
                : kesisimAlani / digerBelgeAlani;
        }

        private void btnSonrakiBelge_Click(object sender, EventArgs e)
        {
            if (bulunanBelgeler.Count == 0)
            {
                MessageBox.Show("Görüntülenecek belge bulunmuyor.");
                return;
            }

            aktifBelgeIndex =
                (aktifBelgeIndex + 1) % bulunanBelgeler.Count;

            AktifBelgeyiGosterVeOku();
        }

        private void btnOncekiBelge_Click(object sender, EventArgs e)
        {
            if (bulunanBelgeler.Count == 0)
            {
                MessageBox.Show("Görüntülenecek belge bulunmuyor.");
                return;
            }

            aktifBelgeIndex--;

            if (aktifBelgeIndex < 0)
                aktifBelgeIndex = bulunanBelgeler.Count - 1;

            AktifBelgeyiGosterVeOku();
        }

        private void AktifBelgeyiGosterVeOku()
        {
            if (bulunanBelgeler.Count == 0)
                return;

            Mat belge = bulunanBelgeler[aktifBelgeIndex];

            ResmiGoster(belge);

            string metin = ocrService.ReadText(belge);
            metin = ocrCleaner.Temizle(metin);

            var belgeBilgi = documentParser.Parse(metin);

            // Kimlik ve adres belgelerindeki kiþisel bilgiler forma aktarýlmaz.
            // OCR metni ekranda gösterilmez ve veritabanýna kaydedilmez.
            aktifBelgeKimlikVeyaAdres =
                KimlikVeyaAdresBelgesiMi(belgeBilgi.BelgeTuru, metin);

            if (aktifBelgeKimlikVeyaAdres)
            {
                FormAlanlariniTemizle();
                aktifBelgeKimlikVeyaAdres = true;

                if (cmbBelgeTuru.Items.Contains("Kimlik / Adres Belgesi"))
                    cmbBelgeTuru.SelectedItem = "Kimlik / Adres Belgesi";

                rtbAciklama.Clear();

                lblBelgeSirasi.Text =
                    $"Belge {aktifBelgeIndex + 1} / {bulunanBelgeler.Count}";

                MessageBox.Show(
                    "Kimlik / adres belgesi algýlandý.\n" +
                    "Kiþisel bilgiler otomatik olarak doldurulmadý. " +
                    "Lütfen belge bilgilerini manuel giriniz.",
                    "Manuel Bilgi Giriþi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            txtBelgeAdi.Text = belgeBilgi.BelgeAdi;

            cmbBelgeTuru.SelectedItem =
                cmbBelgeTuru.Items.Contains(belgeBilgi.BelgeTuru)
                    ? belgeBilgi.BelgeTuru
                    : "Diðer";

            txtKisiKurum.Text = belgeBilgi.KisiKurum;
            rtbAciklama.Text = belgeBilgi.Aciklama;

            lblBelgeSirasi.Text =
                $"Belge {aktifBelgeIndex + 1} / {bulunanBelgeler.Count}";
        }

        private static bool KimlikVeyaAdresBelgesiMi(string belgeTuru, string ocrMetni)
        {
            if (string.Equals(
                    belgeTuru,
                    "Kimlik / Adres Belgesi",
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string metin = (ocrMetni ?? string.Empty).ToUpperInvariant();

            return metin.Contains("T.C. KÝMLÝK") ||
                   metin.Contains("TÜRKÝYE CUMHURÝYETÝ KÝMLÝK") ||
                   metin.Contains("KÝMLÝK KARTI") ||
                   metin.Contains("IDENTITY CARD") ||
                   metin.Contains("YERLEÞÝM YERÝ") ||
                   metin.Contains("ADRES BELGESÝ");
        }

        #endregion

        #region Form alanlarý ve kaydetme

        private void FormAlanlariniTemizle()
        {
            aktifBelgeKimlikVeyaAdres = false;

            txtBelgeAdi.Clear();
            txtKisiKurum.Clear();
            rtbAciklama.Clear();

            if (cmbBelgeTuru.Items.Count > 0)
                cmbBelgeTuru.SelectedIndex = 0;
        }

        private bool BelgeBilgileriGecerliMi()
        {
            if (string.IsNullOrWhiteSpace(txtBelgeAdi.Text))
            {
                MessageBox.Show("Lütfen belge adýný giriniz.");
                return false;
            }

            if (cmbBelgeTuru.SelectedItem == null)
            {
                MessageBox.Show("Lütfen belge türünü seçiniz.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtKisiKurum.Text))
            {
                MessageBox.Show("Lütfen kiþi/kurum bilgisini giriniz.");
                return false;
            }

            return true;
        }


        #endregion

        #region Ýsteðe baðlý manuel görüntü araçlarý

        private void btnGoruntuIyilestir_Click(object sender, EventArgs e)
        {
            if (!AktifResimVarMi())
                return;

            GeriAlIcinSakla();

            string rapor;
            using Mat sonuc = enhancer.Enhance(aktifResim!, out rapor);

            ResmiGoster(sonuc);
            MessageBox.Show(rapor, "Akýllý Görüntü Ýyileþtirme");
        }

        private void btnKenarTespit_Click(object sender, EventArgs e)
        {
            if (!AktifResimVarMi())
                return;

            GeriAlIcinSakla();

            using Mat gri = new Mat();

            if (aktifResim!.Channels() == 3)
                Cv2.CvtColor(aktifResim, gri, ColorConversionCodes.BGR2GRAY);
            else
                aktifResim.CopyTo(gri);

            using Mat kenarlar = new Mat();
            Cv2.Canny(gri, kenarlar, 50, 150);

            ResmiGoster(kenarlar);
        }

        private void btnManuelGriTon_Click(object sender, EventArgs e)
        {
            if (!AktifResimVarMi())
                return;

            if (aktifResim!.Channels() == 1)
            {
                MessageBox.Show("Resim zaten gri tonlu.");
                return;
            }

            GeriAlIcinSakla();

            using Mat gri = new Mat();
            Cv2.CvtColor(aktifResim, gri, ColorConversionCodes.BGR2GRAY);

            ResmiGoster(gri);
        }

        private void btnManuelKontrast_Click(object sender, EventArgs e)
        {
            if (!AktifResimVarMi())
                return;

            GeriAlIcinSakla();

            using Mat sonuc = imageEditor.IncreaseContrast(aktifResim!);
            ResmiGoster(sonuc);
        }

        private void btnManuelGurultu_Click(object sender, EventArgs e)
        {
            if (!AktifResimVarMi())
                return;

            GeriAlIcinSakla();

            using Mat sonuc = imageEditor.RemoveNoise(aktifResim!);
            ResmiGoster(sonuc);
        }

        private void btnManuelKeskin_Click(object sender, EventArgs e)
        {
            if (!AktifResimVarMi())
                return;

            GeriAlIcinSakla();

            using Mat sonuc = imageEditor.Sharpen(aktifResim!);
            ResmiGoster(sonuc);
        }

        private void btnGeriAl_Click(object sender, EventArgs e)
        {
            if (oncekiResim == null || oncekiResim.Empty())
            {
                MessageBox.Show("Geri alýnacak iþlem bulunmuyor.");
                return;
            }

            ResmiGoster(oncekiResim);

            oncekiResim.Dispose();
            oncekiResim = null;
        }

        private void btnSifirla_Click(object sender, EventArgs e)
        {
            if (orijinalResim == null || orijinalResim.Empty())
            {
                MessageBox.Show("Önce resim seçiniz.");
                return;
            }

            ResmiGoster(orijinalResim);
            FormAlanlariniTemizle();
        }

        #endregion

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            BelgeleriTemizle();

            orijinalResim?.Dispose();
            aktifResim?.Dispose();
            oncekiResim?.Dispose();
            pictureBoxBelge.Image?.Dispose();
            yoloService.Dispose();

            base.OnFormClosed(e);
        }

        private string AktifBelgeyiDosyayaKaydet()
        {
            if (aktifResim == null || aktifResim.Empty())
                throw new InvalidOperationException("Kaydedilecek belge bulunmuyor.");

            string klasorYolu = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "ArsivBelgeleri");

            Directory.CreateDirectory(klasorYolu);

            string guvenliBelgeAdi = string.Join(
                "_",
                txtBelgeAdi.Text.Split(Path.GetInvalidFileNameChars()));

            if (string.IsNullOrWhiteSpace(guvenliBelgeAdi))
                guvenliBelgeAdi = "Belge";

            string dosyaAdi =
                $"{DateTime.Now:yyyyMMdd_HHmmss}_{guvenliBelgeAdi}.png";

            string tamYol = Path.Combine(klasorYolu, dosyaAdi);

            Cv2.ImWrite(tamYol, aktifResim);

            return tamYol;
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (!BelgeBilgileriGecerliMi())
                return;

            if (aktifResim == null || aktifResim.Empty())
            {
                MessageBox.Show("Kaydedilecek belge bulunmuyor.");
                return;
            }

            try
            {
                string dosyaYolu = AktifBelgeyiDosyayaKaydet();

                var belge = new DocumentRecord
                {
                    BelgeAdi = txtBelgeAdi.Text.Trim(),
                    BelgeTuru = cmbBelgeTuru.Text,
                    KisiKurum = txtKisiKurum.Text.Trim(),
                    Aciklama = rtbAciklama.Text.Trim(),
                    OcrMetni = aktifBelgeKimlikVeyaAdres
                        ? string.Empty
                        : rtbAciklama.Text.Trim(),
                    DosyaYolu = dosyaYolu,
                    KayitTarihi = DateTime.Now
                };

                int belgeId = databaseService.BelgeEkle(belge);

                MessageBox.Show(
                    $"Belge baþarýyla kaydedildi.\nBelge No: {belgeId}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Belge kaydedilirken hata oluþtu:\n{ex.Message}");
            }
        }
    }
}