using System;
using System.Drawing;
using System.Windows.Forms;

namespace BelgeArsivlemeSistemi.Forms
{
    public partial class FormAna : Form
    {
        private Form? aktifForm;

        public FormAna()
        {
            InitializeComponent();
        }

        private void FormAna_Load(object sender, EventArgs e)
        {
            MenuButonlariniSifirla();

            btnBelgeEkle.BackColor = Color.RoyalBlue;
            btnBelgeEkle.ForeColor = Color.White;

            FormuPaneldeAc(new Form1());
        }

        private void FormuPaneldeAc(Form form)
        {
            if (aktifForm != null)
            {
                aktifForm.Close();
                aktifForm.Dispose();
            }

            aktifForm = form;

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            pnlIcerik.Controls.Clear();
            pnlIcerik.Controls.Add(form);
            pnlIcerik.Tag = form;

            form.BringToFront();
            form.Show();
        }



        private void btnArsivAra_Click(object sender, EventArgs e)
        {
            MenuButonlariniSifirla();

            btnArsivAra.BackColor = Color.RoyalBlue;
            btnArsivAra.ForeColor = Color.White;

            FormuPaneldeAc(new FormArama());
        }

        private void btnHakkinda_Click(object sender, EventArgs e)
        {
            MenuButonlariniSifirla();

            btnHakkinda.BackColor = Color.RoyalBlue;
            btnHakkinda.ForeColor = Color.White;

            MessageBox.Show(
                "Belge Arşivleme Sistemi",
                "Hakkında");
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MenuButonlariniSifirla()
        {
            btnBelgeEkle.BackColor = Color.White;
            btnBelgeEkle.ForeColor = Color.Black;

            btnArsivAra.BackColor = Color.White;
            btnArsivAra.ForeColor = Color.Black;

            btnHakkinda.BackColor = Color.White;
            btnHakkinda.ForeColor = Color.Black;
        }

        private void btnBelgeEkle_Click(object sender, EventArgs e)
        {
            MenuButonlariniSifirla();

            btnBelgeEkle.BackColor = Color.RoyalBlue;
            btnBelgeEkle.ForeColor = Color.White;

            FormuPaneldeAc(new Form1());
        }

        private void pnlIcerik_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}