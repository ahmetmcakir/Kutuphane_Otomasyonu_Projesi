using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KutuphaneOtomasyon.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KutuphaneOtomasyon
{
    public partial class Yonetici : Form
    {
        public Yonetici()
        {
            InitializeComponent();
        }

        
        OleDbConnection con;
        OleDbDataAdapter da;
        DataSet ds;
        void griddoldur()
        {
            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=Kutuphane.accdb");
            da = new OleDbDataAdapter("Select * from Uyeler", con);
            ds = new DataSet();
            con.Open();
            da.Fill(ds, "Uyeler");
            dataGridView1.DataSource = ds.Tables["Uyeler"];
            con.Close();
        }
        private void ListeleKitaplar()
        {
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sorgu = "SELECT * FROM Uyeler";
                    OleDbDataAdapter da = new OleDbDataAdapter(sorgu, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kullanıcı listesi yüklenemedi:\n" + ex.Message);
                }
            }
        }
        private void Yonetici_Load(object sender, EventArgs e)
        {
            ListeleKitaplar();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            OrtakIslemler.Cikis(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            KitaplarTablosu kitaplarTablosu = new KitaplarTablosu();
            kitaplarTablosu.Show();
            Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            EmanetlerTablosu emanetlerTablosu = new EmanetlerTablosu();
            emanetlerTablosu.Show();
            Hide();
        }
        //Ekle butonu
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


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kayıt sırasında hata oluştu:\n" + ex.Message);
                }
            }
            griddoldur();
        }


        //Sil butonu
        private void button2_Click_1(object sender, EventArgs e)
        {

            string girilenUyeID = textBox5.Text.Trim();

            if (string.IsNullOrEmpty(girilenUyeID))
            {
                MessageBox.Show("Lütfen silmek istediğiniz kullanıcının ID'sini giriniz.");
                return;
            }

            if (!int.TryParse(girilenUyeID, out int uyeID))
            {
                MessageBox.Show("Geçerli bir Üye ID giriniz.");
                return;
            }

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    OleDbCommand kontrol = new OleDbCommand("SELECT COUNT(*) FROM Uyeler WHERE UyeID = ?", conn);
                    kontrol.Parameters.Add("UyeID", OleDbType.Integer).Value = uyeID;

                    int sayi = Convert.ToInt32(kontrol.ExecuteScalar());

                    if (sayi == 0)
                    {
                        MessageBox.Show("Bu ID'ye sahip bir kullanıcı bulunamadı.");
                        return;
                    }

                    OleDbCommand sil = new OleDbCommand("DELETE FROM Uyeler WHERE UyeID = ?", conn);
                    sil.Parameters.Add("UyeID", OleDbType.Integer).Value = uyeID;
                    sil.ExecuteNonQuery();

                    MessageBox.Show("Kullanıcı başarıyla silindi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme işlemi sırasında hata oluştu:\n" + ex.Message);
                }
            }

            griddoldur();
        }

        private void dataGridView1_CellEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            textBox5.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBox6.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            textBox7.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
        }
        //Güncelle buttonu
        private void button3_Click_1(object sender, EventArgs e)
        {
            string girilenUyeID = textBox5.Text.Trim();
            string ad = textBox1.Text.Trim();
            string soyad = textBox2.Text.Trim();
            string kullaniciAdi = textBox3.Text.Trim();
            string sifre = textBox4.Text.Trim();
            string telefon = textBox6.Text.Trim();
            string email = textBox7.Text.Trim();

            if (string.IsNullOrEmpty(girilenUyeID) || string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(soyad) ||
                string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre) || string.IsNullOrEmpty(telefon) ||
                string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurunuz.");
                return;
            }

            if (!int.TryParse(girilenUyeID, out int uyeID))
            {
                MessageBox.Show("Geçerli bir Üye ID giriniz.");
                return;
            }

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    OleDbCommand kontrol = new OleDbCommand("SELECT COUNT(*) FROM Uyeler WHERE UyeID = ?", conn);
                    kontrol.Parameters.Add("UyeID", OleDbType.Integer).Value = uyeID;

                    int sayi = Convert.ToInt32(kontrol.ExecuteScalar());

                    if (sayi == 0)
                    {
                        MessageBox.Show("Bu ID'ye sahip bir kullanıcı bulunamadı.");
                        return;
                    }

                    OleDbCommand guncelle = new OleDbCommand(
                        "UPDATE Uyeler SET Ad = ?, Soyad = ?, KullaniciAdi = ?, Sifre = ?, Telefon = ?, Email = ? WHERE UyeID = ?", conn);

                    guncelle.Parameters.Add("Ad", OleDbType.VarChar).Value = ad;
                    guncelle.Parameters.Add("Soyad", OleDbType.VarChar).Value = soyad;
                    guncelle.Parameters.Add("KullaniciAdi", OleDbType.VarChar).Value = kullaniciAdi;
                    guncelle.Parameters.Add("Sifre", OleDbType.VarChar).Value = sifre;
                    guncelle.Parameters.Add("Telefon", OleDbType.VarChar).Value = telefon;
                    guncelle.Parameters.Add("Email", OleDbType.VarChar).Value = email;
                    guncelle.Parameters.Add("UyeID", OleDbType.Integer).Value = uyeID;

                    guncelle.ExecuteNonQuery();

                    MessageBox.Show("Kullanıcı bilgileri başarıyla güncellendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Güncelleme sırasında hata oluştu:\n" + ex.Message);
                }
            }
            griddoldur();




        }

       

       
    }
    
}

