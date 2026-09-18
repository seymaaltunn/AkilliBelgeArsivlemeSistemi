



namespace BelgeArsivlemeSistemi
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnResimSec = new Button();
            pictureBoxBelge = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            txtBelgeAdi = new TextBox();
            txtKisiKurum = new TextBox();
            cmbBelgeTuru = new ComboBox();
            rtbAciklama = new RichTextBox();
            label1 = new Label();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            btnKaydet = new Button();
            lblBelgeSirasi = new Label();
            btnSonrakiBelge = new Button();
            btnOncekiBelge = new Button();
            btnBelgeBul = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxBelge).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // btnResimSec
            // 
            btnResimSec.Location = new Point(6, 26);
            btnResimSec.Name = "btnResimSec";
            btnResimSec.Size = new Size(150, 29);
            btnResimSec.TabIndex = 0;
            btnResimSec.Text = "Resim Seç";
            btnResimSec.UseVisualStyleBackColor = true;
            btnResimSec.Click += btnResimSec_Click;
            // 
            // pictureBoxBelge
            // 
            pictureBoxBelge.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxBelge.Location = new Point(22, 37);
            pictureBoxBelge.Name = "pictureBoxBelge";
            pictureBoxBelge.Size = new Size(375, 282);
            pictureBoxBelge.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxBelge.TabIndex = 5;
            pictureBoxBelge.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(30, 35);
            label2.Name = "label2";
            label2.Size = new Size(74, 20);
            label2.TabIndex = 7;
            label2.Text = "Belge Adı";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(30, 71);
            label3.Name = "label3";
            label3.Size = new Size(80, 20);
            label3.TabIndex = 8;
            label3.Text = "Belge Türü";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(30, 142);
            label4.Name = "label4";
            label4.Size = new Size(70, 20);
            label4.TabIndex = 9;
            label4.Text = "Açıklama";
            // 
            // txtBelgeAdi
            // 
            txtBelgeAdi.Location = new Point(180, 32);
            txtBelgeAdi.Name = "txtBelgeAdi";
            txtBelgeAdi.Size = new Size(220, 27);
            txtBelgeAdi.TabIndex = 10;
            // 
            // txtKisiKurum
            // 
            txtKisiKurum.Location = new Point(180, 102);
            txtKisiKurum.Name = "txtKisiKurum";
            txtKisiKurum.Size = new Size(220, 27);
            txtKisiKurum.TabIndex = 11;
            // 
            // cmbBelgeTuru
            // 
            cmbBelgeTuru.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBelgeTuru.FormattingEnabled = true;
            cmbBelgeTuru.Location = new Point(180, 68);
            cmbBelgeTuru.Name = "cmbBelgeTuru";
            cmbBelgeTuru.Size = new Size(220, 28);
            cmbBelgeTuru.TabIndex = 12;
           
            // 
            // rtbAciklama
            // 
            rtbAciklama.Location = new Point(30, 177);
            rtbAciklama.Name = "rtbAciklama";
            rtbAciklama.Size = new Size(370, 398);
            rtbAciklama.TabIndex = 13;
            rtbAciklama.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 109);
            label1.Name = "label1";
            label1.Size = new Size(81, 20);
            label1.TabIndex = 14;
            label1.Text = "Kişi/Kurum";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(rtbAciklama);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(txtKisiKurum);
            groupBox1.Controls.Add(cmbBelgeTuru);
            groupBox1.Controls.Add(txtBelgeAdi);
            groupBox1.Location = new Point(456, 22);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(418, 598);
            groupBox1.TabIndex = 15;
            groupBox1.TabStop = false;
            groupBox1.Text = "Belge Bilgileri";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(pictureBoxBelge);
            groupBox2.Location = new Point(18, 292);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(409, 328);
            groupBox2.TabIndex = 16;
            groupBox2.TabStop = false;
            groupBox2.Text = "Belge Önizleme";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnKaydet);
            groupBox3.Controls.Add(lblBelgeSirasi);
            groupBox3.Controls.Add(btnSonrakiBelge);
            groupBox3.Controls.Add(btnOncekiBelge);
            groupBox3.Controls.Add(btnBelgeBul);
            groupBox3.Controls.Add(btnResimSec);
            groupBox3.Location = new Point(40, 22);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(222, 231);
            groupBox3.TabIndex = 17;
            groupBox3.TabStop = false;
            groupBox3.Text = "İşlemler";
            // 
            // btnKaydet
            // 
            btnKaydet.Location = new Point(6, 186);
            btnKaydet.Name = "btnKaydet";
            btnKaydet.Size = new Size(150, 29);
            btnKaydet.TabIndex = 22;
            btnKaydet.Text = "Kaydet";
            btnKaydet.UseVisualStyleBackColor = true;
            btnKaydet.Click += btnKaydet_Click;
            // 
            // lblBelgeSirasi
            // 
            lblBelgeSirasi.AutoSize = true;
            lblBelgeSirasi.Location = new Point(6, 109);
            lblBelgeSirasi.Name = "lblBelgeSirasi";
            lblBelgeSirasi.Size = new Size(81, 20);
            lblBelgeSirasi.TabIndex = 21;
            lblBelgeSirasi.Text = "Belge 0 / 0";
           
            // btnSonrakiBelge
            // 
            btnSonrakiBelge.Location = new Point(118, 142);
            btnSonrakiBelge.Name = "btnSonrakiBelge";
            btnSonrakiBelge.Size = new Size(94, 29);
            btnSonrakiBelge.TabIndex = 20;
            btnSonrakiBelge.Text = "Sonraki Belge";
            btnSonrakiBelge.UseVisualStyleBackColor = true;
            btnSonrakiBelge.Click += btnSonrakiBelge_Click;
            // 
            // btnOncekiBelge
            // 
            btnOncekiBelge.Location = new Point(6, 142);
            btnOncekiBelge.Name = "btnOncekiBelge";
            btnOncekiBelge.Size = new Size(94, 29);
            btnOncekiBelge.TabIndex = 19;
            btnOncekiBelge.Text = "Önceki Belge";
            btnOncekiBelge.UseVisualStyleBackColor = true;
            btnOncekiBelge.Click += btnOncekiBelge_Click;
            // 
            // btnBelgeBul
            // 
            btnBelgeBul.Location = new Point(6, 62);
            btnBelgeBul.Name = "btnBelgeBul";
            btnBelgeBul.Size = new Size(150, 29);
            btnBelgeBul.TabIndex = 7;
            btnBelgeBul.Text = "Belgeyi Bul";
            btnBelgeBul.UseVisualStyleBackColor = true;
            btnBelgeBul.Click += btnBelgeBul_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(893, 632);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Belge Arşivleme Sistemi";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxBelge).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnResimSec;
        private PictureBox pictureBoxBelge;
        private Label lblBaslik;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox txtBelgeAdi;
        private TextBox txtKisiKurum;
        private ComboBox cmbBelgeTuru;
        private RichTextBox rtbAciklama;
        private Label label1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Button btnBelgeBul;
        private Button btnOncekiBelge;
        private Button btnSonrakiBelge;
        private Label lblBelgeSirasi;
        private Button btnKaydet;
    }
}
