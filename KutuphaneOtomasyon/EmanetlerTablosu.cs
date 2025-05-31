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

namespace KutuphaneOtomasyon
{
    public partial class EmanetlerTablosu : Form
    {
        public EmanetlerTablosu()
        {
            InitializeComponent();
        }
        private void EmanetleriListele()
        {
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sorgu = "SELECT E.EmanetID, U.Ad & ' ' & U.Soyad AS UyeAdi, K.KitapAdi, E.AlisTarihi, E.IadeTarihi, IIF(E.TeslimEdildiMi = True, 'Evet', 'Hayır') AS TeslimDurumu FROM (Emanetler AS E INNER JOIN Uyeler AS U ON E.UyeID = U.UyeID) INNER JOIN Kitaplar AS K ON E.KitapID = K.KitapID";

                OleDbDataAdapter da = new OleDbDataAdapter(sorgu, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OrtakIslemler.Cikis(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            KitaplarTablosu kitaplarTablosu = new KitaplarTablosu();
            kitaplarTablosu.Show();
            Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Yonetici yonetici = new Yonetici();
            yonetici.Show();
            Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            EmanetleriListele();
        }



       

        
    }
}
