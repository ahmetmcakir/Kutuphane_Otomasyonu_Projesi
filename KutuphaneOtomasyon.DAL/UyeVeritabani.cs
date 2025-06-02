using System;
using System.Data;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;

namespace KutuphaneOtomasyon.DAL
{
    public class UyeVeritabani
    {
        private readonly string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

        // ✅ Rastgele salt üreten metot (şifre güvenliği için gerekli)
        public string SaltUret()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        // ✅ Salt'lı şifre hash'leme metodu
        public string SifreHashle(string sifre, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(sifre + salt);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public DataTable UyeleriGetir()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sorgu = "SELECT * FROM Uyeler";
                OleDbDataAdapter da = new OleDbDataAdapter(sorgu, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool KullaniciAdiVarMi(string kullaniciAdi)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT COUNT(*) FROM Uyeler WHERE KullaniciAdi = ?", conn);
                cmd.Parameters.Add("?", OleDbType.VarChar).Value = kullaniciAdi;
                int sayi = Convert.ToInt32(cmd.ExecuteScalar());
                return sayi > 0;
            }
        }

        public bool UyeVarMi(int uyeID)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT COUNT(*) FROM Uyeler WHERE UyeID = ?", conn);
                cmd.Parameters.Add("?", OleDbType.Integer).Value = uyeID;
                int sayi = Convert.ToInt32(cmd.ExecuteScalar());
                return sayi > 0;
            }
        }

        public void UyeEkle(Uye uye)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand ekle = new OleDbCommand(
                    "INSERT INTO Uyeler (Ad, Soyad, KullaniciAdi, Sifre, Salt, Yetki, Telefon, Email, KayitTarihi) " +
                    "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)", conn);

                ekle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Ad;
                ekle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Soyad;
                ekle.Parameters.Add("?", OleDbType.VarChar).Value = uye.KullaniciAdi;
                ekle.Parameters.Add("?", OleDbType.LongVarChar).Value = uye.Sifre;
                ekle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Salt;
                ekle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Yetki;
                ekle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Telefon;
                ekle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Email;
                ekle.Parameters.Add("?", OleDbType.Date).Value = uye.KayitTarihi;

                ekle.ExecuteNonQuery();
            }
        }

        public void UyeSil(int uyeID)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand sil = new OleDbCommand("DELETE FROM Uyeler WHERE UyeID = ?", conn);
                sil.Parameters.Add("?", OleDbType.Integer).Value = uyeID;
                sil.ExecuteNonQuery();
            }
        }

        public void UyeGuncelle(Uye uye)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand guncelle = new OleDbCommand(
                    "UPDATE Uyeler SET Ad = ?, Soyad = ?, KullaniciAdi = ?, Sifre = ?, Salt = ?, Yetki = ?, Telefon = ?, Email = ? WHERE UyeID = ?", conn);

                guncelle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Ad;
                guncelle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Soyad;
                guncelle.Parameters.Add("?", OleDbType.VarChar).Value = uye.KullaniciAdi;
                guncelle.Parameters.Add("?", OleDbType.LongVarChar).Value = uye.Sifre;
                guncelle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Salt;
                guncelle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Yetki;
                guncelle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Telefon;
                guncelle.Parameters.Add("?", OleDbType.VarChar).Value = uye.Email;
                guncelle.Parameters.Add("?", OleDbType.Integer).Value = uye.UyeID;

                guncelle.ExecuteNonQuery();
            }
        }
    }
}
