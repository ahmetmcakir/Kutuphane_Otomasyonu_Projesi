using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using static KutuphaneOtomasyon.Form1;

namespace KutuphaneOtomasyon
{
    public partial class Girismenu : Form
    {
        private string kullaniciAdi;

        public Girismenu(string kullanici)
        {
            InitializeComponent();
            kullaniciAdi = kullanici;
        }

        private void Girismenu_Load_1(object sender, EventArgs e)
        {
            ListeleKitaplar();
            ListeleKitaplar2();
            label1.Text = kullaniciAdi;
        }
     
        private void ListeleKitaplar()
        {
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sorgu = "SELECT * FROM Kitaplar";
                    OleDbDataAdapter da = new OleDbDataAdapter(sorgu, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvKitaplar.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kitap listesi yüklenemedi:\n" + ex.Message);
                }
            }
        }
        private void ListeleKitaplar2()
        {
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sorgu = "SELECT * FROM Kitaplar";
                    OleDbDataAdapter da = new OleDbDataAdapter(sorgu, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEmanetler.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kitap listesi yüklenemedi:\n" + ex.Message);
                }
            }
        }
        private int GetUyeID(OleDbConnection conn)
        {
            // KullaniciAdi üzerinden sorgulama
            OleDbCommand uyeKomut = new OleDbCommand("SELECT UyeID FROM Uyeler WHERE KullaniciAdi = ?", conn);
            uyeKomut.Parameters.Add("?", OleDbType.VarChar).Value = kullaniciAdi;
            object uyeIDObj = uyeKomut.ExecuteScalar();

            if (uyeIDObj == null)
                throw new Exception("Kullanıcıya ait üye kaydı bulunamadı.");

            return Convert.ToInt32(uyeIDObj);
        }

        private void dgvEmanetler_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvEmanetler.Rows)
            {
                string durum = row.Cells["Durum"].Value?.ToString();

                if (durum == "Geciken")
                    row.DefaultCellStyle.BackColor = Color.Red;
                else if (durum == "Teslim Edildi")
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                else if (durum == "Devam Eden")
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
            }
        }

        // Kitap Ödünç Al
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dgvKitaplar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen ödünç almak için bir kitap seçin.");
                return;
            }

            int kitapID = Convert.ToInt32(dgvKitaplar.SelectedRows[0].Cells["KitapID"].Value);
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    int uyeID = GetUyeID(conn);

                    OleDbCommand emanetEkle = new OleDbCommand(
                        "INSERT INTO Emanetler (UyeID, KitapID, AlisTarihi, IadeTarihi, TeslimEdildiMi) VALUES (?, ?, ?, ?, ?)", conn);
                    emanetEkle.Parameters.Add("?", OleDbType.Integer).Value = uyeID;
                    emanetEkle.Parameters.Add("?", OleDbType.Integer).Value = kitapID;
                    emanetEkle.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;
                    emanetEkle.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now.AddDays(14);
                    emanetEkle.Parameters.Add("?", OleDbType.Boolean).Value = false;
                    emanetEkle.ExecuteNonQuery();

                    OleDbCommand stokAzalt = new OleDbCommand("UPDATE Kitaplar SET Adet = Adet - 1 WHERE KitapID = ?", conn);
                    stokAzalt.Parameters.Add("?", OleDbType.Integer).Value = kitapID;
                    stokAzalt.ExecuteNonQuery();

                    MessageBox.Show("Kitap başarıyla ödünç alındı.");
                    ListeleKitaplar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata:\n" + ex.Message);
                }
            }
        }

        // Kitap İade Et
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (dgvKitaplar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen iade etmek için bir kitap seçin.");
                return;
            }

            int kitapID = Convert.ToInt32(dgvKitaplar.SelectedRows[0].Cells["KitapID"].Value);
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    int uyeID = GetUyeID(conn);

                    OleDbCommand kontrol = new OleDbCommand(
                        "SELECT EmanetID FROM Emanetler WHERE UyeID = ? AND KitapID = ? AND TeslimEdildiMi = false", conn);
                    kontrol.Parameters.Add("?", OleDbType.Integer).Value = uyeID;
                    kontrol.Parameters.Add("?", OleDbType.Integer).Value = kitapID;

                    object emanetIDObj = kontrol.ExecuteScalar();

                    if (emanetIDObj == null)
                    {
                        MessageBox.Show("Bu kitabı zaten iade etmişsiniz veya ödünç almamışsınız.");
                        return;
                    }

                    int emanetID = Convert.ToInt32(emanetIDObj);

                    OleDbCommand teslimEt = new OleDbCommand(
                        "UPDATE Emanetler SET TeslimEdildiMi = true WHERE EmanetID = ?", conn);
                    teslimEt.Parameters.Add("?", OleDbType.Integer).Value = emanetID;
                    teslimEt.ExecuteNonQuery();

                    OleDbCommand stokArtir = new OleDbCommand(
                        "UPDATE Kitaplar SET Adet = Adet + 1 WHERE KitapID = ?", conn);
                    stokArtir.Parameters.Add("?", OleDbType.Integer).Value = kitapID;
                    stokArtir.ExecuteNonQuery();

                    MessageBox.Show("Kitap iade edildi.");
                    ListeleKitaplar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("İade sırasında hata:\n" + ex.Message);
                }
            }
        }

        // Emanetleri Listele
        private void button3_Click_1(object sender, EventArgs e)
        {
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    int uyeID = GetUyeID(conn);

                    string sorgu = @"
                        SELECT 
                            K.KitapAdi, 
                            K.Yazar, 
                            E.AlisTarihi, 
                            E.IadeTarihi, 
                            E.TeslimEdildiMi,
                            IIf(E.TeslimEdildiMi = true, 'Teslim Edildi', 
                            IIf(E.IadeTarihi < Date(), 'Geciken', 'Devam Eden')) AS Durum
                        FROM 
                            Emanetler E
                        INNER JOIN 
                            Kitaplar K ON E.KitapID = K.KitapID
                        WHERE 
                            E.UyeID = ?";

                    OleDbCommand cmd = new OleDbCommand(sorgu, conn);
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = uyeID;
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEmanetler.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Emanetleri getirirken hata:\n" + ex.Message);
                }
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            OrtakIslemler.Cikis(this);
        }

       
    }
}
