using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using KutuphaneOtomasyon;

namespace KutuphaneOtomasyon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static class OrtakIslemler
        {
            public static void Cikis(Form mevcutForm)
            {
                Form1 form1 = new Form1();
                form1.Show();
                mevcutForm.Hide();
            }
        }
        //Giriş Butonu
        private void button1_Click_1(object sender, EventArgs e)
        {
            string kullaniciAdi = textBox1.Text.Trim();
            string sifre = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre giriniz.");
                return;
            }

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            try
            {
                using (OleDbConnection baglanti = new OleDbConnection(connectionString))
                {
                    baglanti.Open();

                    string sorgu = "SELECT COUNT(*) FROM Uyeler WHERE KullaniciAdi = ? AND Sifre = ?";

                    using (OleDbCommand komut = new OleDbCommand(sorgu, baglanti))
                    {
                        komut.Parameters.Add("?", OleDbType.VarChar).Value = kullaniciAdi;
                        komut.Parameters.Add("?", OleDbType.VarChar).Value = sifre;

                        int sonuc = Convert.ToInt32(komut.ExecuteScalar());

                        if (sonuc > 0)
                        {
                            MessageBox.Show("Giriş başarılı!");

                            Girismenu giris = new Girismenu(textBox1.Text.Trim());
                            giris.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı adı veya şifre hatalı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        //Üye ol
        private void button2_Click_1(object sender, EventArgs e)
        {
            UyeOl uyeForm = new UyeOl();
            uyeForm.Show();
            this.Hide();
        }
        //Yönetici Giriş
        private void button3_Click_1(object sender, EventArgs e)
        {
            string Yönetici_Adi = "admin";
            string Yönetici_Sifre = "password";
            string gelen_ad = textBox1.Text;
            string gelen_sifre = textBox2.Text;
            if (Yönetici_Adi == gelen_ad && Yönetici_Sifre == gelen_sifre)
            {
                Yonetici yonetici = new Yonetici();
                yonetici.Show();
                Hide();
            }
            else
                MessageBox.Show("Kullanıcı Adı Veya Şifre Hatalı");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

       

       

       
    }
}
