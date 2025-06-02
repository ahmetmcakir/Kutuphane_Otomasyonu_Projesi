using System;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using KutuphaneOtomasyon.DAL;

namespace KutuphaneOtomasyon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Ortak çıkış işlemi (başka formlardan çağrılır)
        public static class OrtakIslemler
        {
            public static void Cikis(Form mevcutForm)
            {
                Form1 form1 = new Form1();
                form1.Show();
                mevcutForm.Hide();
            }
        }

        // Şifreyi salt ile birleştirip SHA256 ile hash'ler
        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string combined = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(combined);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder result = new StringBuilder();
                foreach (byte b in hash)
                {
                    result.Append(b.ToString("x2"));
                }

                return result.ToString();
            }
        }

        // Giriş butonuna tıklanınca çalışır
        private void button1_Click_1(object sender, EventArgs e)
        {
            string kullaniciAdi = textBox1.Text.Trim();
            string sifre = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre giriniz.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            try
            {
                using (OleDbConnection baglanti = new OleDbConnection(connectionString))
                {
                    baglanti.Open();

                    string sorgu = "SELECT Yetki, Salt, Sifre FROM Uyeler WHERE KullaniciAdi = ?";

                    using (OleDbCommand komut = new OleDbCommand(sorgu, baglanti))
                    {
                        komut.Parameters.Add("?", OleDbType.VarChar).Value = kullaniciAdi;

                        using (OleDbDataReader reader = komut.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string yetki = reader["Yetki"].ToString();
                                string salt = reader["Salt"].ToString();
                                string storedHash = reader["Sifre"].ToString();

                                string enteredHash = HashPassword(sifre, salt);

                                if (enteredHash == storedHash)
                                {
                                    MessageBox.Show("Giriş başarılı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    if (yetki == "Kullanici")
                                    {
                                        Girismenu giris = new Girismenu(kullaniciAdi);
                                        giris.Show();
                                        this.Hide();
                                    }
                                    else if (yetki == "Yonetici")
                                    {
                                        Yonetici yonetici = new Yonetici();
                                        yonetici.Show();
                                        this.Hide();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Geçersiz yetki değeri. Yetki yalnızca 'Kullanici' veya 'Yonetici' olabilir.", "Yetki Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Kullanıcı adı bulunamadı.", "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata mesajını kullanıcıya gösteriyorum
                MessageBox.Show("Giriş sırasında bir hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Log dosyasına hata kaydediyorum
                LogHelper.LogYaz("Form1 - Giriş Hatası:\n" + ex.ToString());
            }
        }

        // "Üye Ol" butonuna tıklanınca çalışır
        private void button2_Click_1(object sender, EventArgs e)
        {
            UyeOl uyeForm = new UyeOl();
            uyeForm.Show();
            this.Hide();
        }
    }
}
