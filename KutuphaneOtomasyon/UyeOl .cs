using System;
using System.Windows.Forms;
using KutuphaneOtomasyon.DAL;
using System.Security.Cryptography;
using System.Text;

namespace KutuphaneOtomasyon
{
    public partial class UyeOl : Form
    {
        public UyeOl()
        {
            InitializeComponent();
            textBox4.PasswordChar = '*'; // Şifreyi gizlemek için yıldız karakteri kullanıyorum
        }

        // Rastgele Salt üretmek için metot
        private string GenerateSalt()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        // Kullanıcının şifresini Salt ile birleştirerek SHA256 ile şifreliyorum
        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string combined = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(combined);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        // "Üye Ol" butonuna basıldığında çalışır
        private void button1_Click_1(object sender, EventArgs e)
        {
            // Giriş bilgilerini textbox'lardan alıyorum
            string ad = textBox1.Text.Trim();
            string soyad = textBox2.Text.Trim();
            string kullaniciAdi = textBox3.Text.Trim();
            string sifre = textBox4.Text.Trim();
            string telefon = textBox6.Text.Trim();
            string email = textBox7.Text.Trim();
            DateTime kayitTarihi = DateTime.Now;
            string yetki = "Kullanici"; // Her kayıt olan kullanıcı normal kullanıcı olur

            // Alanlardan herhangi biri boşsa kullanıcıyı uyarıyorum
            if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad) ||
                string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre) ||
                string.IsNullOrWhiteSpace(telefon) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurunuz.");
                return;
            }

            try
            {
                var uyeVeritabani = new UyeVeritabani();

                // Kullanıcı adı zaten kullanılıyor mu kontrol ediyorum
                if (uyeVeritabani.KullaniciAdiVarMi(kullaniciAdi))
                {
                    MessageBox.Show("Bu kullanıcı adı zaten kullanılıyor.");
                    return;
                }

                // Salt oluşturup şifreyi hashliyorum
                string salt = GenerateSalt();
                string hashedPassword = HashPassword(sifre, salt);

                // Yeni kullanıcı nesnesini oluşturuyorum
                var uye = new Uye
                {
                    Ad = ad,
                    Soyad = soyad,
                    KullaniciAdi = kullaniciAdi,
                    Sifre = hashedPassword,
                    Telefon = telefon,
                    Email = email,
                    KayitTarihi = kayitTarihi,
                    Salt = salt,
                    Yetki = yetki
                };

                // Veritabanına kullanıcıyı kaydediyorum
                uyeVeritabani.UyeEkle(uye);

                MessageBox.Show("Üyelik başarıyla oluşturuldu!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Giriş ekranına geri dönüyorum
                Form1 form1 = new Form1();
                form1.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }
        }

        // "Geri Dön" butonuna basıldığında giriş formuna döner
        private void button2_Click_1(object sender, EventArgs e)
        {
            OrtakIslemler.Cikis(this);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Boş event – istenirse kaldırılabilir
        }
    }
}
