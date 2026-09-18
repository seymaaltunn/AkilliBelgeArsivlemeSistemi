namespace BelgeArsivlemeSistemi
{
    partial class FormArama
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            lblKayitSayisi = new Label();
            btnBelgeSil = new Button();
            btnBelgeDuzenle = new Button();
            btnBelgAc = new Button();
            btnTemizle = new Button();
            btnAra = new Button();
            lblBelgeAdi = new Label();
            txtBelgeAdi = new TextBox();
            dtpBitis = new DateTimePicker();
            lblBelgeTuru = new Label();
            dtpBaslangic = new DateTimePicker();
            cmbBelgeTuru = new ComboBox();
            lblTarih = new Label();
            txtKisiKurum = new TextBox();
            lblKisiKurum = new Label();
            groupBox2 = new GroupBox();
            dgvBelgeler = new DataGridView();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBelgeler).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblKayitSayisi);
            groupBox1.Controls.Add(btnBelgeSil);
            groupBox1.Controls.Add(btnBelgeDuzenle);
            groupBox1.Controls.Add(btnBelgAc);
            groupBox1.Controls.Add(btnTemizle);
            groupBox1.Controls.Add(btnAra);
            groupBox1.Controls.Add(lblBelgeAdi);
            groupBox1.Controls.Add(txtBelgeAdi);
            groupBox1.Controls.Add(dtpBitis);
            groupBox1.Controls.Add(lblBelgeTuru);
            groupBox1.Controls.Add(dtpBaslangic);
            groupBox1.Controls.Add(cmbBelgeTuru);
            groupBox1.Controls.Add(lblTarih);
            groupBox1.Controls.Add(txtKisiKurum);
            groupBox1.Controls.Add(lblKisiKurum);
            groupBox1.Location = new Point(23, 21);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(755, 353);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Arama Kriterleri";
            groupBox1.Enter += groupBox1_Enter;
            // 
            // lblKayitSayisi
            // 
            lblKayitSayisi.AutoSize = true;
            lblKayitSayisi.Location = new Point(537, 162);
            lblKayitSayisi.Name = "lblKayitSayisi";
            lblKayitSayisi.Size = new Size(97, 20);
            lblKayitSayisi.TabIndex = 16;
            lblKayitSayisi.Text = "Toplam kayıt:";
            // 
            // btnBelgeSil
            // 
            btnBelgeSil.Location = new Point(537, 110);
            btnBelgeSil.Name = "btnBelgeSil";
            btnBelgeSil.Size = new Size(170, 29);
            btnBelgeSil.TabIndex = 15;
            btnBelgeSil.Text = "Belgeyi Sil";
            btnBelgeSil.UseVisualStyleBackColor = true;
            btnBelgeSil.Click += btnBelgeSil_Click;
            // 
            // btnBelgeDuzenle
            // 
            btnBelgeDuzenle.Location = new Point(537, 71);
            btnBelgeDuzenle.Name = "btnBelgeDuzenle";
            btnBelgeDuzenle.Size = new Size(170, 29);
            btnBelgeDuzenle.TabIndex = 14;
            btnBelgeDuzenle.Text = "Belge Düzenleme";
            btnBelgeDuzenle.UseVisualStyleBackColor = true;
            btnBelgeDuzenle.Click += btnBelgeDuzenle_Click;
            // 
            // btnBelgAc
            // 
            btnBelgAc.Location = new Point(537, 26);
            btnBelgAc.Name = "btnBelgAc";
            btnBelgAc.Size = new Size(170, 29);
            btnBelgAc.TabIndex = 13;
            btnBelgAc.Text = "Belge Aç";
            btnBelgAc.UseVisualStyleBackColor = true;
            btnBelgAc.Click += btnBelgAc_Click;
            // 
            // btnTemizle
            // 
            btnTemizle.Location = new Point(467, 306);
            btnTemizle.Name = "btnTemizle";
            btnTemizle.Size = new Size(120, 29);
            btnTemizle.TabIndex = 12;
            btnTemizle.Text = "Temizle";
            btnTemizle.UseVisualStyleBackColor = true;
            btnTemizle.Click += btnTemizle_Click;
            // 
            // btnAra
            // 
            btnAra.Location = new Point(17, 306);
            btnAra.Name = "btnAra";
            btnAra.Size = new Size(120, 29);
            btnAra.TabIndex = 11;
            btnAra.Text = "Ara";
            btnAra.UseVisualStyleBackColor = true;
            btnAra.Click += btnAra_Click;
            // 
            // lblBelgeAdi
            // 
            lblBelgeAdi.AutoSize = true;
            lblBelgeAdi.Location = new Point(17, 30);
            lblBelgeAdi.Name = "lblBelgeAdi";
            lblBelgeAdi.Size = new Size(77, 20);
            lblBelgeAdi.TabIndex = 1;
            lblBelgeAdi.Text = "Belge Adı:";
            // 
            // txtBelgeAdi
            // 
            txtBelgeAdi.Location = new Point(17, 53);
            txtBelgeAdi.Name = "txtBelgeAdi";
            txtBelgeAdi.Size = new Size(180, 27);
            txtBelgeAdi.TabIndex = 5;
            // 
            // dtpBitis
            // 
            dtpBitis.Location = new Point(337, 260);
            dtpBitis.Name = "dtpBitis";
            dtpBitis.Size = new Size(250, 27);
            dtpBitis.TabIndex = 9;
            // 
            // lblBelgeTuru
            // 
            lblBelgeTuru.AutoSize = true;
            lblBelgeTuru.Location = new Point(18, 96);
            lblBelgeTuru.Name = "lblBelgeTuru";
            lblBelgeTuru.Size = new Size(83, 20);
            lblBelgeTuru.TabIndex = 2;
            lblBelgeTuru.Text = "Belge Türü:";
            // 
            // dtpBaslangic
            // 
            dtpBaslangic.Location = new Point(18, 260);
            dtpBaslangic.Name = "dtpBaslangic";
            dtpBaslangic.Size = new Size(250, 27);
            dtpBaslangic.TabIndex = 8;
            // 
            // cmbBelgeTuru
            // 
            cmbBelgeTuru.FormattingEnabled = true;
            cmbBelgeTuru.Location = new Point(17, 119);
            cmbBelgeTuru.Name = "cmbBelgeTuru";
            cmbBelgeTuru.Size = new Size(180, 28);
            cmbBelgeTuru.TabIndex = 7;
            // 
            // lblTarih
            // 
            lblTarih.AutoSize = true;
            lblTarih.Location = new Point(18, 237);
            lblTarih.Name = "lblTarih";
            lblTarih.Size = new Size(88, 20);
            lblTarih.TabIndex = 4;
            lblTarih.Text = "Tarih Aralığı";
            // 
            // txtKisiKurum
            // 
            txtKisiKurum.Location = new Point(17, 185);
            txtKisiKurum.Name = "txtKisiKurum";
            txtKisiKurum.Size = new Size(180, 27);
            txtKisiKurum.TabIndex = 6;
            // 
            // lblKisiKurum
            // 
            lblKisiKurum.AutoSize = true;
            lblKisiKurum.Location = new Point(18, 162);
            lblKisiKurum.Name = "lblKisiKurum";
            lblKisiKurum.Size = new Size(92, 20);
            lblKisiKurum.TabIndex = 3;
            lblKisiKurum.Text = "Kişi / Kurum:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(dgvBelgeler);
            groupBox2.Location = new Point(23, 392);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(755, 185);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Arama Sonuçları";
            // 
            // dgvBelgeler
            // 
            dgvBelgeler.AllowUserToAddRows = false;
            dgvBelgeler.AllowUserToResizeRows = false;
            dgvBelgeler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBelgeler.BackgroundColor = SystemColors.ControlLightLight;
            dgvBelgeler.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBelgeler.Dock = DockStyle.Fill;
            dgvBelgeler.Location = new Point(3, 23);
            dgvBelgeler.MultiSelect = false;
            dgvBelgeler.Name = "dgvBelgeler";
            dgvBelgeler.ReadOnly = true;
            dgvBelgeler.RowHeadersVisible = false;
            dgvBelgeler.RowHeadersWidth = 51;
            dgvBelgeler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBelgeler.Size = new Size(749, 159);
            dgvBelgeler.TabIndex = 0;
            // 
            // FormArama
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(800, 589);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormArama";
            Text = "Arşivde Ara";
            Load += FormArama_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvBelgeler).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label lblBelgeAdi;
        private TextBox txtBelgeAdi;
        private Label lblBelgeTuru;
        private DateTimePicker dtpBaslangic;
        private ComboBox cmbBelgeTuru;
        private Label lblTarih;
        private TextBox txtKisiKurum;
        private Label lblKisiKurum;
        private DateTimePicker dtpBitis;
        private Button btnAra;
        private Button btnTemizle;
        private GroupBox groupBox2;
        private DataGridView dgvBelgeler;
        private Button btnBelgeSil;
        private Button btnBelgeDuzenle;
        private Button btnBelgAc;
        private Label lblKayitSayisi;
    }
}