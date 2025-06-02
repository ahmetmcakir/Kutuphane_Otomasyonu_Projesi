using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using KutuphaneOtomasyon.DAL;
using KutuphaneOtomasyon.BLL;

namespace KutuphaneOtomasyon
{
    public partial class Girismenu : Form
    {
        private readonly string kullaniciAdi;

        public Girismenu(string kullanici)
        {
            InitializeComponent();
            kullaniciAdi = kullanici;
        }

        // Form yüklendiğinde kitapları listeliyorum
        private void Girismenu_Load_1(object sender, EventArgs e)
        {
            try
            {
                dgvKitaplar.DataSource = new Veritabani().KitaplariGetir();
                dgvEmanetler.DataSource = new DataTable(); // Başlangıçta boş olsun
                label1.Text = kullaniciAdi;
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
            }

        }

        // Giriş yapan kullanıcının UyeID’sini veritabanından alıyorum
        private int GetUyeID(OleDbConnection conn)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT UyeID FROM Uyeler WHERE KullaniciAdi = ?", conn);
            cmd.Parameters.Add("?", OleDbType.VarChar).Value = kullaniciAdi;

            object result = cmd.ExecuteScalar();
            if (result == null)
                throw new Exception("Kullanıcıya ait üye kaydı bulunamadı.");

            return Convert.ToInt32(result);
        }

        // DataGridView renklendirme (emanet durumu için)
        private void dgvEmanetler_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvEmanetler.Rows)
            {
                string durum = row.Cells["Durum"].Value?.ToString();

                if (durum != null)
                {
                    if (durum.Contains("Geciken"))
                        row.DefaultCellStyle.BackColor = Color.Red;
                    else if (durum == "Teslim Edildi")
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    else if (durum == "Devam Eden")
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }
        }

        // Kitap ödünç alma işlemi
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dgvKitaplar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen ödünç almak için bir kitap seçin.");
                return;
            }

            int kitapID = Convert.ToInt32(dgvKitaplar.SelectedRows[0].Cells["KitapID"].Value);

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb"))
            {
                try
                {
                    conn.Open();
                    int uyeID = GetUyeID(conn);

                    new Veritabani().KitapOduncAl(conn, uyeID, kitapID);
                    MessageBox.Show("Kitap başarıyla ödünç alındı.");

                    // Kitap listesini güncelliyorum
                    dgvKitaplar.DataSource = new Veritabani().KitaplariGetir();
                }
                catch (Exception ex)
                {
                    LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                    MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
                }

            }
        }

        // Kitap iade etme işlemi
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (dgvKitaplar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen iade etmek için bir kitap seçin.");
                return;
            }

            int kitapID = Convert.ToInt32(dgvKitaplar.SelectedRows[0].Cells["KitapID"].Value);

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb"))
            {
                try
                {
                    conn.Open();
                    int uyeID = GetUyeID(conn);

                    new Veritabani().KitapIadeEt(conn, uyeID, kitapID);
                    MessageBox.Show("Kitap başarıyla iade edildi.");

                    dgvKitaplar.DataSource = new Veritabani().KitaplariGetir();
                }
                catch (Exception ex)
                {
                    LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                    MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
                }

            }
        }

        // Kullanıcının emanet kitaplarını listeleme + gecikme kontrolü
        private void button3_Click_1(object sender, EventArgs e)
        {
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb"))
            {
                try
                {
                    conn.Open();
                    int uyeID = GetUyeID(conn);

                    // Veritabanından emanet verileri çekiliyor
                    var dt = new Veritabani().EmanetleriGetir(uyeID);

                    // Geciken kitapları ve cezaları işliyorum
                    var guncellenmis = EmanetIslemleri.CezaHesapla(dt);

                    // Listeyi DataGridView'e yansıtıyorum
                    dgvEmanetler.DataSource = guncellenmis;
                }
                catch (Exception ex)
                {
                    LogHelper.LogYaz(ex.ToString()); // hatayı dosyaya yaz
                    MessageBox.Show("Kayıt sırasında hata:\n" + ex.Message);
                }

            }
        }

        // Çıkış butonuna basıldığında giriş formuna dönülür
        private void button4_Click_1(object sender, EventArgs e)
        {
            OrtakIslemler.Cikis(this);
        }
    }
}
