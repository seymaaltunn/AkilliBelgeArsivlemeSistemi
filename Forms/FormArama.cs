using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BelgeArsivlemeSistemi.Database;
using BelgeArsivlemeSistemi.Models;
using System.Diagnostics;
using System.IO;

namespace BelgeArsivlemeSistemi
{
    public partial class FormArama : Form
    {
        private readonly DatabaseService databaseService =
    new DatabaseService();
        public FormArama()
        {
            InitializeComponent();
        }

        private void btnAramaEkrani_Click(object sender, EventArgs e)
        {
            FormArama aramaFormu = new FormArama();
            aramaFormu.ShowDialog();
        }

        private void FormArama_Load(object sender, EventArgs e)
        {
            cmbBelgeTuru.Items.Clear();
            cmbBelgeTuru.Items.Add("");
            cmbBelgeTuru.Items.Add("Kurumsal / İdari Belge");
            cmbBelgeTuru.Items.Add("Meclis / Encümen Kararı");
            cmbBelgeTuru.Items.Add("Gelen / Giden Evrak");
            cmbBelgeTuru.Items.Add("Genelge / Yönetmelik");
            cmbBelgeTuru.Items.Add("İmar / Şehircilik Belgesi");
            cmbBelgeTuru.Items.Add("İmar Planı / Pafta");
            cmbBelgeTuru.Items.Add("Ruhsat");
            cmbBelgeTuru.Items.Add("Mimari Proje");
            cmbBelgeTuru.Items.Add("Mali / Satın Alma Belgesi");
            cmbBelgeTuru.Items.Add("İhale Dosyası");
            cmbBelgeTuru.Items.Add("Fatura / Mali Evrak");
            cmbBelgeTuru.Items.Add("Dilekçe / Başvuru");
            cmbBelgeTuru.Items.Add("Kimlik / Adres Belgesi");
            cmbBelgeTuru.Items.Add("Diğer");

            cmbBelgeTuru.SelectedIndex = 0;

            dtpBaslangic.Value = DateTime.Today.AddYears(-1);
            dtpBitis.Value = DateTime.Today;

            BelgeleriListele();
        }

        private void BelgeleriListele()
        {
            dgvBelgeler.DataSource = null;
            dgvBelgeler.DataSource = databaseService.BelgeleriGetir();

            if (dgvBelgeler.Columns["OcrMetni"] != null)
                dgvBelgeler.Columns["OcrMetni"].Visible = false;

            if (dgvBelgeler.Columns["Aciklama"] != null)
                dgvBelgeler.Columns["Aciklama"].Visible = false;

            if (dgvBelgeler.Columns["DosyaYolu"] != null)
                dgvBelgeler.Columns["DosyaYolu"].Visible = false;

            lblKayitSayisi.Text =
                $"Toplam kayıt: {dgvBelgeler.Rows.Count}";
        }
        private void btnTumunuListele_Click(object sender, EventArgs e)
        {
            BelgeleriListele();
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            var sonuc = databaseService.BelgeAra(
                txtBelgeAdi.Text,
                cmbBelgeTuru.Text,
                txtKisiKurum.Text,
                dtpBaslangic.Value,
                dtpBitis.Value);

            dgvBelgeler.DataSource = null;
            dgvBelgeler.DataSource = sonuc;
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtBelgeAdi.Clear();
            txtKisiKurum.Clear();
            cmbBelgeTuru.SelectedIndex = 0;

            dtpBaslangic.Value = DateTime.Today.AddYears(-1);
            dtpBitis.Value = DateTime.Today;

            BelgeleriListele();
        }

        private void btnBelgeSil_Click(object sender, EventArgs e)
        {
            if (dgvBelgeler.CurrentRow == null)
            {
                MessageBox.Show(
                    "Lütfen silmek istediğiniz belgeyi tablodan seçiniz.",
                    "Belge Seçilmedi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(
                    dgvBelgeler.CurrentRow.Cells["Id"].Value?.ToString(),
                    out int belgeId))
            {
                MessageBox.Show(
                    "Seçilen belgenin kayıt numarası okunamadı.",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            string belgeAdi =
                dgvBelgeler.CurrentRow.Cells["BelgeAdi"].Value?.ToString()
                ?? "Seçili belge";

            string dosyaYolu =
                dgvBelgeler.CurrentRow.Cells["DosyaYolu"].Value?.ToString()
                ?? string.Empty;

            DialogResult cevap = MessageBox.Show(
                $"'{belgeAdi}' adlı belge silinecek.\n\n" +
                "Bu işlem geri alınamaz. Devam edilsin mi?",
                "Belgeyi Sil",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (cevap != DialogResult.Yes)
                return;

            try
            {
                bool silindi = databaseService.BelgeSil(belgeId);

                if (!silindi)
                {
                    MessageBox.Show(
                        "Belge veritabanında bulunamadı veya silinemedi.",
                        "Silme Başarısız",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Veritabanı kaydı silindikten sonra arşivdeki görsel dosyayı da kaldır.
                if (!string.IsNullOrWhiteSpace(dosyaYolu) &&
                    File.Exists(dosyaYolu))
                {
                    try
                    {
                        File.Delete(dosyaYolu);
                    }
                    catch
                    {
                        MessageBox.Show(
                            "Veritabanı kaydı silindi ancak belge dosyası diskten silinemedi.",
                            "Kısmi Silme",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }

                BelgeleriListele();

                MessageBox.Show(
                    "Belge başarıyla silindi.",
                    "Silme Tamamlandı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Belge silinirken hata oluştu:\n{ex.Message}",
                    "Silme Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void btnBelgeDuzenle_Click(object sender, EventArgs e)
        {
            if (dgvBelgeler.CurrentRow == null)
            {
                MessageBox.Show(
                    "Lütfen düzenlemek istediğiniz belgeyi tablodan seçiniz.",
                    "Belge Seçilmedi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(
                    dgvBelgeler.CurrentRow.Cells["Id"].Value?.ToString(),
                    out int belgeId))
            {
                MessageBox.Show(
                    "Seçilen belgenin kayıt numarası okunamadı.",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            string mevcutBelgeAdi =
                dgvBelgeler.CurrentRow.Cells["BelgeAdi"].Value?.ToString()
                ?? string.Empty;

            string mevcutBelgeTuru =
                dgvBelgeler.CurrentRow.Cells["BelgeTuru"].Value?.ToString()
                ?? string.Empty;

            string mevcutKisiKurum =
                dgvBelgeler.CurrentRow.Cells["KisiKurum"].Value?.ToString()
                ?? string.Empty;

            string mevcutAciklama =
                dgvBelgeler.CurrentRow.Cells["Aciklama"].Value?.ToString()
                ?? string.Empty;

            using Form duzenlemeFormu = new Form
            {
                Text = "Belge Düzenleme",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ClientSize = new Size(460, 390)
            };

            Label lblAdi = new Label
            {
                Text = "Belge Adı:",
                Left = 20,
                Top = 20,
                AutoSize = true
            };

            TextBox txtAdi = new TextBox
            {
                Left = 20,
                Top = 45,
                Width = 410,
                Text = mevcutBelgeAdi
            };

            Label lblTur = new Label
            {
                Text = "Belge Türü:",
                Left = 20,
                Top = 85,
                AutoSize = true
            };

            ComboBox cmbTur = new ComboBox
            {
                Left = 20,
                Top = 110,
                Width = 410,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            foreach (object item in cmbBelgeTuru.Items)
            {
                string deger = item?.ToString() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(deger))
                    cmbTur.Items.Add(deger);
            }

            if (cmbTur.Items.Contains(mevcutBelgeTuru))
                cmbTur.SelectedItem = mevcutBelgeTuru;
            else if (cmbTur.Items.Count > 0)
                cmbTur.SelectedIndex = 0;

            Label lblKisi = new Label
            {
                Text = "Kişi / Kurum:",
                Left = 20,
                Top = 150,
                AutoSize = true
            };

            TextBox txtKisi = new TextBox
            {
                Left = 20,
                Top = 175,
                Width = 410,
                Text = mevcutKisiKurum
            };

            Label lblAciklama = new Label
            {
                Text = "Açıklama:",
                Left = 20,
                Top = 215,
                AutoSize = true
            };

            TextBox txtAciklama = new TextBox
            {
                Left = 20,
                Top = 240,
                Width = 410,
                Height = 80,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Text = mevcutAciklama
            };

            Button btnKaydet = new Button
            {
                Text = "Kaydet",
                Left = 230,
                Top = 340,
                Width = 95,
                DialogResult = DialogResult.OK
            };

            Button btnIptal = new Button
            {
                Text = "İptal",
                Left = 335,
                Top = 340,
                Width = 95,
                DialogResult = DialogResult.Cancel
            };

            duzenlemeFormu.Controls.AddRange(new Control[]
            {
                lblAdi,
                txtAdi,
                lblTur,
                cmbTur,
                lblKisi,
                txtKisi,
                lblAciklama,
                txtAciklama,
                btnKaydet,
                btnIptal
            });

            duzenlemeFormu.AcceptButton = btnKaydet;
            duzenlemeFormu.CancelButton = btnIptal;

            if (duzenlemeFormu.ShowDialog(this) != DialogResult.OK)
                return;

            if (string.IsNullOrWhiteSpace(txtAdi.Text))
            {
                MessageBox.Show(
                    "Belge adı boş bırakılamaz.",
                    "Eksik Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (cmbTur.SelectedItem == null)
            {
                MessageBox.Show(
                    "Belge türü seçilmelidir.",
                    "Eksik Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtKisi.Text))
            {
                MessageBox.Show(
                    "Kişi / kurum bilgisi boş bırakılamaz.",
                    "Eksik Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var belge = new DocumentRecord
                {
                    Id = belgeId,
                    BelgeAdi = txtAdi.Text.Trim(),
                    BelgeTuru = cmbTur.Text,
                    KisiKurum = txtKisi.Text.Trim(),
                    Aciklama = txtAciklama.Text.Trim()
                };

                bool guncellendi = databaseService.BelgeGuncelle(belge);

                if (!guncellendi)
                {
                    MessageBox.Show(
                        "Belge bulunamadı veya güncellenemedi.",
                        "Güncelleme Başarısız",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                BelgeleriListele();

                MessageBox.Show(
                    "Belge bilgileri başarıyla güncellendi.",
                    "Güncelleme Tamamlandı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Belge güncellenirken hata oluştu:\n{ex.Message}",
                    "Güncelleme Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnBelgAc_Click(object sender, EventArgs e)
        {
            if (dgvBelgeler.CurrentRow == null)
            {
                MessageBox.Show(
                    "Lütfen açmak istediğiniz belgeyi seçiniz.",
                    "Belge Seçilmedi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string? dosyaYolu =
                dgvBelgeler.CurrentRow.Cells["DosyaYolu"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(dosyaYolu))
            {
                MessageBox.Show(
                    "Seçilen belgenin dosya yolu bulunamadı.",
                    "Dosya Yolu Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (!File.Exists(dosyaYolu))
            {
                MessageBox.Show(
                    $"Belge dosyası bulunamadı.\n\nDosya yolu:\n{dosyaYolu}",
                    "Dosya Bulunamadı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = dosyaYolu,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Belge açılırken hata oluştu:\n\n{ex.Message}",
                    "Belge Açma Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}