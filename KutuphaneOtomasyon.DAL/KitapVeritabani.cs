using System;
using System.Data;
using System.Data.OleDb;

namespace KutuphaneOtomasyon.DAL
{
    public class KitapVeritabani
    {
        private readonly string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

        public DataTable KitaplariGetir()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string sorgu = "SELECT * FROM Kitaplar";
                OleDbDataAdapter da = new OleDbDataAdapter(sorgu, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public void KitapEkle(Kitap kitap)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(
                    "INSERT INTO Kitaplar (KitapAdi, Yazar, Yayinevi, BasimYili, ISBN, Adet) VALUES (?, ?, ?, ?, ?, ?)", conn);

                cmd.Parameters.AddWithValue("?", kitap.KitapAdi);
                cmd.Parameters.AddWithValue("?", kitap.Yazar);
                cmd.Parameters.AddWithValue("?", kitap.Yayinevi);
                cmd.Parameters.AddWithValue("?", kitap.BasimYili);
                cmd.Parameters.AddWithValue("?", kitap.ISBN);
                cmd.Parameters.AddWithValue("?", kitap.Adet);
                cmd.ExecuteNonQuery();
            }
        }

        public void KitapSil(int kitapID)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("DELETE FROM Kitaplar WHERE KitapID = ?", conn);
                cmd.Parameters.AddWithValue("?", kitapID);
                cmd.ExecuteNonQuery();
            }
        }

        public void KitapGuncelle(Kitap kitap)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(
                    "UPDATE Kitaplar SET KitapAdi = ?, Yazar = ?, Yayinevi = ?, BasimYili = ?, ISBN = ?, Adet = ? WHERE KitapID = ?", conn);

                cmd.Parameters.AddWithValue("?", kitap.KitapAdi);
                cmd.Parameters.AddWithValue("?", kitap.Yazar);
                cmd.Parameters.AddWithValue("?", kitap.Yayinevi);
                cmd.Parameters.AddWithValue("?", kitap.BasimYili);
                cmd.Parameters.AddWithValue("?", kitap.ISBN);
                cmd.Parameters.AddWithValue("?", kitap.Adet);
                cmd.Parameters.AddWithValue("?", kitap.KitapID);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // DTO sınıfı
    public class Kitap
    {
        public int KitapID { get; set; }
        public string KitapAdi { get; set; }
        public string Yazar { get; set; }
        public string Yayinevi { get; set; }
        public int BasimYili { get; set; }
        public string ISBN { get; set; }
        public int Adet { get; set; }
    }
}
