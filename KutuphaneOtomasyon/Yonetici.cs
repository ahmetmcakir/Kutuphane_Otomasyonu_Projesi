using System;
using System.Data;
using System.Windows.Forms;
using KutuphaneOtomasyon.DAL;
using static KutuphaneOtomasyon.Form1;
using KutuphaneOtomasyon.DAL; // Yedekleme sınıfını kullanmak için

namespace KutuphaneOtomasyon
{
    public partial class Yonetici : Form
    {
        private readonly UyeVeritabani uyeVeritabani;

        public Yonetici()
        {
            InitializeComponent();
            uyeVeritabani = new UyeVeritabani();
            textBox4.PasswordChar = '*';
            cmbYetki.Items.AddRange(new[] { "Kullanici", "Yonetici" });
            cmbYetki.SelectedIndex = 0;
        }

        private void Yonetici_Load(object sender, EventArgs e)
        {
            UyeleriListele();
        }

        private void UyeleriListele()
        {
            try
            {
                dataGridView1.DataSource = uyeVeritabani.UyeleriGetir();
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string ad = textBox1.Text.Trim();
            string soyad = textBox2.Text.Trim();
            string kullaniciAdi = textBox3.Text.Trim();
            string sifre = textBox4.Text.Trim();
            string telefon = textBox6.Text.Trim();
            string email = textBox7.Text.Trim();
            DateTime kayitTarihi = DateTime.Now;
            string yetki = cmbYetki.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad) ||
                string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre) ||
                string.IsNullOrWhiteSpace(telefon) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(yetki))
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurunuz.");
                return;
            }

            try
            {
                if (uyeVeritabani.KullaniciAdiVarMi(kullaniciAdi))
                {
                    MessageBox.Show("Bu kullanıcı adı zaten kullanılıyor.");
                    return;
                }

                string salt = uyeVeritabani.SaltUret();
                string hashedPassword = uyeVeritabani.SifreHashle(sifre, salt);

                var uye = new Uye
                {
                    Ad = ad,
                    Soyad = soyad,
                    KullaniciAdi = kullaniciAdi,
                    Sifre = hashedPassword,
                    Salt = salt,
                    Yetki = yetki,
                    Telefon = telefon,
                    Email = email,
                    KayitTarihi = kayitTarihi
                };

                uyeVeritabani.UyeEkle(uye);

                MessageBox.Show("Üye başarıyla eklendi.");
                UyeleriListele();
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox5.Text.Trim(), out int uyeID))
            {
                MessageBox.Show("Geçerli bir Üye ID giriniz.");
                return;
            }

            try
            {
                if (!uyeVeritabani.UyeVarMi(uyeID))
                {
                    MessageBox.Show("Bu ID'ye sahip bir kullanıcı bulunamadı.");
                    return;
                }

                uyeVeritabani.UyeSil(uyeID);
                MessageBox.Show("Kullanıcı silindi.");
                UyeleriListele();
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox5.Text.Trim(), out int uyeID))
            {
                MessageBox.Show("Geçerli bir Üye ID giriniz.");
                return;
            }

            string ad = textBox1.Text.Trim();
            string soyad = textBox2.Text.Trim();
            string kullaniciAdi = textBox3.Text.Trim();
            string sifre = textBox4.Text.Trim();
            string telefon = textBox6.Text.Trim();
            string email = textBox7.Text.Trim();
            string yetki = cmbYetki.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad) ||
                string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre) ||
                string.IsNullOrWhiteSpace(telefon) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(yetki))
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurunuz.");
                return;
            }

            try
            {
                if (!uyeVeritabani.UyeVarMi(uyeID))
                {
                    MessageBox.Show("Bu ID'ye sahip bir kullanıcı bulunamadı.");
                    return;
                }

                string salt = uyeVeritabani.SaltUret();
                string hashedPassword = uyeVeritabani.SifreHashle(sifre, salt);

                var uye = new Uye
                {
                    UyeID = uyeID,
                    Ad = ad,
                    Soyad = soyad,
                    KullaniciAdi = kullaniciAdi,
                    Sifre = hashedPassword,
                    Salt = salt,
                    Yetki = yetki,
                    Telefon = telefon,
                    Email = email
                };

                uyeVeritabani.UyeGuncelle(uye);

                MessageBox.Show("Kullanıcı bilgileri güncellendi.");
                UyeleriListele();
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        private void dataGridView1_CellEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            textBox5.Text = dataGridView1.CurrentRow.Cells["UyeID"].Value.ToString();
            textBox1.Text = dataGridView1.CurrentRow.Cells["Ad"].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells["Soyad"].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells["KullaniciAdi"].Value.ToString();
            textBox4.Text = "********"; // Şifre gösterilmiyor
            textBox6.Text = dataGridView1.CurrentRow.Cells["Telefon"].Value.ToString();
            textBox7.Text = dataGridView1.CurrentRow.Cells["Email"].Value.ToString();
            cmbYetki.SelectedItem = dataGridView1.CurrentRow.Cells["Yetki"].Value.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OrtakIslemler.Cikis(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new KitaplarTablosu().Show();
            Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new EmanetlerTablosu().Show();
            Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {


            string kaynakDosya = "Kutuphane.accdb";

            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Yedeklerin kaydedileceği klasörü seçin";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string hedefYol = YedeklemeIslemleri.VeritabaniYedekle(kaynakDosya, dialog.SelectedPath);
                        MessageBox.Show("Yedekleme başarılı:\n" + hedefYol, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Yedekleme sırasında hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            VeritabaniGeriYukleForm veritabaniGeriYukleForm =new VeritabaniGeriYukleForm();
            veritabaniGeriYukleForm.Show();
            Hide();
        }
    }
}
