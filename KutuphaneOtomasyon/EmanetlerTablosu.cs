using System;
using System.Data;
using System.Windows.Forms;
using KutuphaneOtomasyon.DAL; // Veritabanı işlemleri için
using static KutuphaneOtomasyon.Form1;

namespace KutuphaneOtomasyon
{
    public partial class EmanetlerTablosu : Form
    {
        private readonly Veritabani veritabani;

        public EmanetlerTablosu()
        {
            InitializeComponent();
            veritabani = new Veritabani(); // DAL sınıfından nesne oluşturuyorum
        }

        // Emanetleri listeleyen metot
        private void EmanetleriListele()
        {
            try
            {
                // DAL katmanındaki metodu çağırarak emanetleri çekiyorum
                DataTable dt = veritabani.TumEmanetleriGetir();
                dataGridView1.DataSource = dt; // Gelen veriyi tabloya bağlıyorum
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        // "Çıkış Yap" butonu
        private void button1_Click(object sender, EventArgs e)
        {
            OrtakIslemler.Cikis(this);
        }

        // "Kitaplar" formuna geç
        private void button2_Click(object sender, EventArgs e)
        {
            new KitaplarTablosu().Show();
            this.Hide();
        }

        // "Yönetici" formuna geç
        private void button3_Click(object sender, EventArgs e)
        {
            new Yonetici().Show();
            this.Hide();
        }

        // "Listele" butonu
        private void button4_Click(object sender, EventArgs e)
        {
            EmanetleriListele(); // Listeleme metodu çağrılır
        }
    }
}
