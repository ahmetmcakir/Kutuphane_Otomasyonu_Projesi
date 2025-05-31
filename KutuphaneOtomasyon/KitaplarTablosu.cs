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
    public partial class KitaplarTablosu : Form
    {
        OleDbConnection con;
        OleDbDataAdapter da;
        DataSet ds;
        public KitaplarTablosu()
        {
            InitializeComponent();
        }
        void griddoldur2()
        {
            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=Kutuphane.accdb");
            da = new OleDbDataAdapter("Select * from Kitaplar", con);
            ds = new DataSet();
            con.Open();
            da.Fill(ds, "Kitaplar");
            dataGridView1.DataSource = ds.Tables["Kitaplar"];
            con.Close();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            Yonetici yonetici = new Yonetici();
            yonetici.Show();
            Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            EmanetlerTablosu emanetlerTablosu = new EmanetlerTablosu();
            emanetlerTablosu.Show();
            Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OrtakIslemler.Cikis(this);
        }

        
        //Ekle Buttonu
        private void button1_Click_1(object sender, EventArgs e)
        {
            string kitapAdi = textBox2.Text.Trim();
            string yazar = textBox3.Text.Trim();
            string yayinevi = textBox4.Text.Trim();
            string basimYiliText = textBox5.Text.Trim();
            string isbn = textBox6.Text.Trim();
            string adetText = textBox7.Text.Trim();

            if (kitapAdi == "" || yazar == "" || yayinevi == "" || basimYiliText == "" || isbn == "" || adetText == "")
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurunuz.");
                return;
            }

            if (!int.TryParse(basimYiliText, out int basimYili) || !int.TryParse(adetText, out int adet))
            {
                MessageBox.Show("Basım yılı ve adet sayısı geçerli bir sayı olmalıdır.");
                return;
            }

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    OleDbCommand ekle = new OleDbCommand(
                        "INSERT INTO Kitaplar (KitapAdi, Yazar, Yayinevi, BasimYili, ISBN, Adet) " +
                        "VALUES (?, ?, ?, ?, ?, ?)", conn);

                    ekle.Parameters.AddWithValue("KitapAdi", kitapAdi);
                    ekle.Parameters.AddWithValue("Yazar", yazar);
                    ekle.Parameters.AddWithValue("Yayinevi", yayinevi);
                    ekle.Parameters.AddWithValue("BasimYili", basimYili);
                    ekle.Parameters.AddWithValue("ISBN", isbn);
                    ekle.Parameters.AddWithValue("Adet", adet);

                    ekle.ExecuteNonQuery();

                    MessageBox.Show("Kitap başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ekleme sırasında hata oluştu:\n" + ex.Message);
                }
            }

            griddoldur2();
        }
        //Sil Buttonu
        private void button2_Click_1(object sender, EventArgs e)
        {
            string kitapIDText = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(kitapIDText))
            {
                MessageBox.Show("Lütfen silmek istediğiniz kitabın KitapID değerini giriniz.");
                return;
            }

            if (!int.TryParse(kitapIDText, out int kitapID))
            {
                MessageBox.Show("Geçerli bir KitapID giriniz (sayı olmalıdır).");
                return;
            }

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    OleDbCommand sil = new OleDbCommand("DELETE FROM Kitaplar WHERE KitapID = ?", conn);
                    sil.Parameters.AddWithValue("KitapID", kitapID);

                    int etkilenen = sil.ExecuteNonQuery();

                    if (etkilenen > 0)
                    {
                        MessageBox.Show("Kitap başarıyla silindi.");
                    }
                    else
                    {
                        MessageBox.Show("Bu ID'ye sahip bir kitap bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme işlemi sırasında hata oluştu:\n" + ex.Message);
                }
            }

            griddoldur2();
        }

        private void KitaplarTablosu_Load(object sender, EventArgs e)
        {
            griddoldur2();
        }
        //Güncelle buttonu
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text.Trim(), out int kitapID))
            {
                MessageBox.Show("Geçerli bir KitapID giriniz.");
                return;
            }

            string kitapAdi = textBox2.Text.Trim();
            string yazar = textBox3.Text.Trim();
            string yayinevi = textBox4.Text.Trim();
            string basimYiliText = textBox5.Text.Trim();
            string isbn = textBox6.Text.Trim();
            string adetText = textBox7.Text.Trim();

            if (kitapAdi == "" || yazar == "" || yayinevi == "" || basimYiliText == "" || isbn == "" || adetText == "")
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurunuz.");
                return;
            }

            if (!int.TryParse(basimYiliText, out int basimYili) || !int.TryParse(adetText, out int adet))
            {
                MessageBox.Show("Basım Yılı ve Adet sayısal değerler olmalıdır.");
                return;
            }

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    OleDbCommand guncelle = new OleDbCommand(
                        "UPDATE Kitaplar SET KitapAdi = ?, Yazar = ?, Yayinevi = ?, BasimYili = ?, ISBN = ?, Adet = ? WHERE KitapID = ?", conn);

                    guncelle.Parameters.AddWithValue("KitapAdi", kitapAdi);
                    guncelle.Parameters.AddWithValue("Yazar", yazar);
                    guncelle.Parameters.AddWithValue("Yayinevi", yayinevi);
                    guncelle.Parameters.AddWithValue("BasimYili", basimYili);
                    guncelle.Parameters.AddWithValue("ISBN", isbn);
                    guncelle.Parameters.AddWithValue("Adet", adet);
                    guncelle.Parameters.AddWithValue("KitapID", kitapID);

                    int etkilenen = guncelle.ExecuteNonQuery();

                    if (etkilenen > 0)
                    {
                        MessageBox.Show("Kitap bilgileri başarıyla güncellendi.");
                    }
                    else
                    {
                        MessageBox.Show("Bu ID'ye sahip bir kitap bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Güncelleme sırasında hata oluştu:\n" + ex.Message);
                }
            }
            griddoldur2();
        }

        private void dataGridView1_CellEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBox5.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            textBox6.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            textBox7.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
        }

       

        

        

      

        

       
    }
}
