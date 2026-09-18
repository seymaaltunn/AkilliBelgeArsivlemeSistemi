namespace BelgeArsivlemeSistemi.Forms
{
    partial class FormAna
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
            pnlBaslik = new Panel();
            lblBaslik = new Label();
            pnlMenu = new Panel();
            btnCikis = new Button();
            btnBelgeEkle = new Button();
            btnHakkinda = new Button();
            btnArsivAra = new Button();
            pnlIcerik = new Panel();
            pnlBaslik.SuspendLayout();
            pnlMenu.SuspendLayout();
            SuspendLayout();
            // 
            // pnlBaslik
            // 
            pnlBaslik.BackColor = Color.FromArgb(35, 15, 65);
            pnlBaslik.Controls.Add(lblBaslik);
            pnlBaslik.Dock = DockStyle.Top;
            pnlBaslik.Location = new Point(0, 0);
            pnlBaslik.Name = "pnlBaslik";
            pnlBaslik.Size = new Size(1182, 75);
            pnlBaslik.TabIndex = 0;
            // 
            // lblBaslik
            // 
            lblBaslik.Dock = DockStyle.Fill;
            lblBaslik.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblBaslik.ForeColor = SystemColors.ButtonHighlight;
            lblBaslik.Location = new Point(0, 0);
            lblBaslik.Name = "lblBaslik";
            lblBaslik.Size = new Size(1182, 75);
            lblBaslik.TabIndex = 0;
            lblBaslik.Text = "BELGE ARŞİVLEME SİSTEMİ";
            lblBaslik.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlMenu
            // 
            pnlMenu.BackColor = Color.FromArgb(45, 55, 75);
            pnlMenu.Controls.Add(btnCikis);
            pnlMenu.Controls.Add(btnBelgeEkle);
            pnlMenu.Controls.Add(btnHakkinda);
            pnlMenu.Controls.Add(btnArsivAra);
            pnlMenu.Dock = DockStyle.Left;
            pnlMenu.Location = new Point(0, 75);
            pnlMenu.Name = "pnlMenu";
            pnlMenu.Size = new Size(250, 628);
            pnlMenu.TabIndex = 1;
            // 
            // btnCikis
            // 
            btnCikis.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnCikis.Location = new Point(20, 205);
            btnCikis.Name = "btnCikis";
            btnCikis.Size = new Size(180, 45);
            btnCikis.TabIndex = 3;
            btnCikis.Text = "Çıkış";
            btnCikis.UseVisualStyleBackColor = true;
            btnCikis.Click += btnCikis_Click;
            // 
            // btnBelgeEkle
            // 
            btnBelgeEkle.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnBelgeEkle.Location = new Point(20, 40);
            btnBelgeEkle.Name = "btnBelgeEkle";
            btnBelgeEkle.Size = new Size(180, 45);
            btnBelgeEkle.TabIndex = 0;
            btnBelgeEkle.Text = "Belge Ekle";
            btnBelgeEkle.UseVisualStyleBackColor = true;
            btnBelgeEkle.Click += btnBelgeEkle_Click;
            // 
            // btnHakkinda
            // 
            btnHakkinda.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnHakkinda.Location = new Point(20, 150);
            btnHakkinda.Name = "btnHakkinda";
            btnHakkinda.Size = new Size(180, 45);
            btnHakkinda.TabIndex = 2;
            btnHakkinda.Text = "Hakkında";
            btnHakkinda.UseVisualStyleBackColor = true;
            btnHakkinda.Click += btnHakkinda_Click;
            // 
            // btnArsivAra
            // 
            btnArsivAra.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnArsivAra.Location = new Point(20, 95);
            btnArsivAra.Name = "btnArsivAra";
            btnArsivAra.Size = new Size(180, 45);
            btnArsivAra.TabIndex = 1;
            btnArsivAra.Text = "Arşivde Ara";
            btnArsivAra.UseVisualStyleBackColor = true;
            btnArsivAra.Click += btnArsivAra_Click;
            // 
            // pnlIcerik
            // 
            pnlIcerik.AutoScroll = true;
            pnlIcerik.BackColor = Color.White;
            pnlIcerik.Dock = DockStyle.Fill;
            pnlIcerik.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 162);
            pnlIcerik.Location = new Point(250, 75);
            pnlIcerik.Name = "pnlIcerik";
            pnlIcerik.Size = new Size(932, 628);
            pnlIcerik.TabIndex = 2;
            pnlIcerik.Paint += pnlIcerik_Paint;
            // 
            // FormAna
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1182, 703);
            Controls.Add(pnlIcerik);
            Controls.Add(pnlMenu);
            Controls.Add(pnlBaslik);
            MinimumSize = new Size(1200, 750);
            Name = "FormAna";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Belge Arşivleme Sistemi";
            Load += FormAna_Load;
            pnlBaslik.ResumeLayout(false);
            pnlMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlBaslik;
        private Panel pnlMenu;
        private Panel pnlIcerik;
        private Label lblBaslik;
        private Button btnCikis;
        private Button btnBelgeEkle;
        private Button btnHakkinda;
        private Button btnArsivAra;
    }
}