using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using KutuphaneOtomasyon.DAL;
using static KutuphaneOtomasyon.Form1;

namespace KutuphaneOtomasyon
{
    public partial class KitaplarTablosu : Form
    {
        private readonly KitapVeritabani kitapVeritabani;

        public KitaplarTablosu()
        {
            InitializeComponent();
            kitapVeritabani = new KitapVeritabani(); // DAL sınıfı
        }

        private void KitaplarTablosu_Load(object sender, EventArgs e)
        {
            KitaplariListele();
        }

        private void KitaplariListele()
        {
            try
            {
                dataGridView1.DataSource = kitapVeritabani.KitaplariGetir();
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Ekle
            if (!VeriKontrol(out Kitap kitap)) return;

            try
            {
                kitapVeritabani.KitapEkle(kitap);
                MessageBox.Show("Kitap başarıyla eklendi.");
                KitaplariListele();
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text.Trim(), out int kitapID))
            {
                MessageBox.Show("Geçerli bir KitapID giriniz.");
                return;
            }

            try
            {
                kitapVeritabani.KitapSil(kitapID);
                MessageBox.Show("Kitap silindi.");
                KitaplariListele();
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text.Trim(), out int kitapID))
            {
                MessageBox.Show("Geçerli bir KitapID giriniz.");
                return;
            }

            if (!VeriKontrol(out Kitap kitap)) return;
            kitap.KitapID = kitapID;

            try
            {
                kitapVeritabani.KitapGuncelle(kitap);
                MessageBox.Show("Kitap güncellendi.");
                KitaplariListele();
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        private void dataGridView1_CellEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = dataGridView1.CurrentRow.Cells["KitapID"].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells["KitapAdi"].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells["Yazar"].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells["Yayinevi"].Value.ToString();
            textBox5.Text = dataGridView1.CurrentRow.Cells["BasimYili"].Value.ToString();
            textBox6.Text = dataGridView1.CurrentRow.Cells["ISBN"].Value.ToString();
            textBox7.Text = dataGridView1.CurrentRow.Cells["Adet"].Value.ToString();
        }

        private bool VeriKontrol(out Kitap kitap)
        {
            kitap = null;

            if (!int.TryParse(textBox5.Text.Trim(), out int basimYili) || !int.TryParse(textBox7.Text.Trim(), out int adet))
            {
                MessageBox.Show("Basım yılı ve adet sayısı sayı olmalıdır.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
                return false;
            }

            kitap = new Kitap
            {
                KitapAdi = textBox2.Text.Trim(),
                Yazar = textBox3.Text.Trim(),
                Yayinevi = textBox4.Text.Trim(),
                BasimYili = basimYili,
                ISBN = textBox6.Text.Trim(),
                Adet = adet
            };
            return true;
        }

        private void button4_Click(object sender, EventArgs e) => OrtakIslemler.Cikis(this);
        private void button5_Click(object sender, EventArgs e) => new EmanetlerTablosu().Show(); // geçiş
        private void button6_Click(object sender, EventArgs e) => new Yonetici().Show(); // geçiş
    }
}
