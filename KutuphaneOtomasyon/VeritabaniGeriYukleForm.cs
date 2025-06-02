using KutuphaneOtomasyon.DAL;
using System;
using System.IO;
using System.Windows.Forms;

namespace KutuphaneOtomasyon
{
    public partial class VeritabaniGeriYukleForm : Form
    {
        private readonly string hedefDosya = "Kutuphane.accdb"; // Mevcut veritabanı dosyası

        public VeritabaniGeriYukleForm()
        {
            InitializeComponent();
        }

        private void btnDosyaSec_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Access Veritabanı (*.accdb)|*.accdb";
            openFile.Title = "Yedek Dosyasını Seç";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtDosyaYolu.Text = openFile.FileName;
            }
        }

        private void btnGeriYukle_Click(object sender, EventArgs e)
        {
            string secilenYedek = txtDosyaYolu.Text;

            if (string.IsNullOrWhiteSpace(secilenYedek) || !File.Exists(secilenYedek))
            {
                MessageBox.Show("Lütfen geçerli bir yedek dosyası seçin.");
                return;
            }

            try
            {
                // Önce mevcut veritabanını yedekleyelim
                string eskiYedekAdi = $"Kutuphane_Eski_{DateTime.Now:yyyyMMdd_HHmmss}.accdb";
                File.Copy(hedefDosya, eskiYedekAdi, true);

                // Seçilen yedekle değiştir
                File.Copy(secilenYedek, hedefDosya, true);

                MessageBox.Show("Veritabanı başarıyla geri yüklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            this.Close(); // Formu kapat
        }
    }
}
