using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using static KutuphaneOtomasyon.Form1;

namespace KutuphaneOtomasyon
{
    public partial class UyeOl : Form
    {
        public UyeOl()
        {
            InitializeComponent();
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

            if (ad == "" || soyad == "" || kullaniciAdi == "" || sifre == "" || telefon == "" || email == "")
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurunuz.");
                return;
            }

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    OleDbCommand kontrol = new OleDbCommand("SELECT COUNT(*) FROM Uyeler WHERE KullaniciAdi = ?", conn);
                    kontrol.Parameters.Add("KullaniciAdi", OleDbType.VarChar).Value = kullaniciAdi;

                    int sayi = Convert.ToInt32(kontrol.ExecuteScalar());

                    if (sayi > 0)
                    {
                        MessageBox.Show("Bu kullanıcı adı zaten kullanılıyor.");
                        return;
                    }

                    OleDbCommand ekle = new OleDbCommand(
                        "INSERT INTO Uyeler (Ad, Soyad, KullaniciAdi, Sifre, Telefon, Email, KayitTarihi) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?)", conn);

                    ekle.Parameters.Add("Ad", OleDbType.VarChar).Value = ad;
                    ekle.Parameters.Add("Soyad", OleDbType.VarChar).Value = soyad;
                    ekle.Parameters.Add("KullaniciAdi", OleDbType.VarChar).Value = kullaniciAdi;
                    ekle.Parameters.Add("Sifre", OleDbType.VarChar).Value = sifre;
                    ekle.Parameters.Add("Telefon", OleDbType.VarChar).Value = telefon;
                    ekle.Parameters.Add("Email", OleDbType.VarChar).Value = email;
                    ekle.Parameters.Add("KayitTarihi", OleDbType.Date).Value = kayitTarihi;

                    ekle.ExecuteNonQuery();

                    MessageBox.Show("Üyelik başarıyla oluşturuldu!");

                    Form1 form1 = new Form1();
                    form1.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kayıt sırasında hata oluştu:\n" + ex.Message);
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            OrtakIslemler.Cikis(this);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

       
    }
    
}
