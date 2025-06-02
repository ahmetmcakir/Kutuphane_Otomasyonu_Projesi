using System;
using System.Data;
using System.Data.OleDb;

namespace KutuphaneOtomasyon.DAL
{

    public class Veritabani
    {
        public DataTable TumEmanetleriGetir()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sorgu = @"
            SELECT 
                E.EmanetID, 
                U.Ad & ' ' & U.Soyad AS UyeAdi, 
                K.KitapAdi, 
                E.AlisTarihi, 
                E.IadeTarihi, 
                IIF(E.TeslimEdildiMi = True, 'Evet', 'Hayır') AS TeslimDurumu 
            FROM 
                (Emanetler AS E 
                INNER JOIN Uyeler AS U ON E.UyeID = U.UyeID) 
                INNER JOIN Kitaplar AS K ON E.KitapID = K.KitapID";

                OleDbDataAdapter da = new OleDbDataAdapter(sorgu, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Veritabanı bağlantısı için kullanacağım bağlantı cümlesini tanımladım.
        private readonly string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Kutuphane.accdb";

        // Kitaplar tablosundaki tüm kitapları veritabanından çekmek için bu metodu oluşturdum.
        public DataTable KitaplariGetir()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sorgu = "SELECT * FROM Kitaplar"; // Tüm kitapları seçen SQL sorgusu
                OleDbDataAdapter da = new OleDbDataAdapter(sorgu, conn);
                DataTable dt = new DataTable();
                da.Fill(dt); // Gelen verileri DataTable içine dolduruyorum
                return dt; // Formda göstermek için geriye döndürüyorum
            }
        }

        // Belirli bir üyeye ait emanet kitap bilgilerini çekmek için bu metodu yazdım.
        public DataTable EmanetleriGetir(int uyeID)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // Kitap ve emanet tablosunu birleştirerek gerekli alanları seçiyorum.
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
                        E.UyeID = ?"; // Sadece ilgili üyeye ait kayıtları getiriyorum

                OleDbCommand cmd = new OleDbCommand(sorgu, conn);
                cmd.Parameters.Add("?", OleDbType.Integer).Value = uyeID; // Üye ID’sini parametre olarak ekledim

                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt); // Sorgudan gelen sonuçları tabloya aktarıyorum
                return dt;
            }
        }

        // Üyenin seçtiği kitabı ödünç alabilmesi için veritabanına ekleme ve stok düşürme işlemleri yaptım.
        public void KitapOduncAl(OleDbConnection conn, int uyeID, int kitapID)
        {
            // Emanetler tablosuna yeni kayıt ekliyorum
            OleDbCommand emanetEkle = new OleDbCommand(
                "INSERT INTO Emanetler (UyeID, KitapID, AlisTarihi, IadeTarihi, TeslimEdildiMi) VALUES (?, ?, ?, ?, ?)", conn);
            emanetEkle.Parameters.Add("?", OleDbType.Integer).Value = uyeID;
            emanetEkle.Parameters.Add("?", OleDbType.Integer).Value = kitapID;
            emanetEkle.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;
            emanetEkle.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now.AddDays(14); // 14 gün sonra iade tarihi
            emanetEkle.Parameters.Add("?", OleDbType.Boolean).Value = false; // Henüz teslim edilmedi
            emanetEkle.ExecuteNonQuery();

            // Kitabın stok adedini 1 azaltıyorum
            OleDbCommand stokAzalt = new OleDbCommand("UPDATE Kitaplar SET Adet = Adet - 1 WHERE KitapID = ?", conn);
            stokAzalt.Parameters.Add("?", OleDbType.Integer).Value = kitapID;
            stokAzalt.ExecuteNonQuery();
        }

        // Üyenin kitabı iade etmesi için teslim ve stok arttırma işlemlerini yapıyorum.
        public void KitapIadeEt(OleDbConnection conn, int uyeID, int kitapID)
        {
            // Önce kitabın emanet kaydının olup olmadığını kontrol ediyorum
            OleDbCommand kontrol = new OleDbCommand(
                "SELECT EmanetID FROM Emanetler WHERE UyeID = ? AND KitapID = ? AND TeslimEdildiMi = false", conn);
            kontrol.Parameters.Add("?", OleDbType.Integer).Value = uyeID;
            kontrol.Parameters.Add("?", OleDbType.Integer).Value = kitapID;

            object emanetIDObj = kontrol.ExecuteScalar();
            if (emanetIDObj == null)
                throw new Exception("Bu kitabı zaten iade etmişsiniz veya ödünç almamışsınız.");

            int emanetID = Convert.ToInt32(emanetIDObj);

            // Emanet kaydını teslim edildi olarak işaretliyorum
            OleDbCommand teslimEt = new OleDbCommand(
                "UPDATE Emanetler SET TeslimEdildiMi = true WHERE EmanetID = ?", conn);
            teslimEt.Parameters.Add("?", OleDbType.Integer).Value = emanetID;
            teslimEt.ExecuteNonQuery();

            // Kitabın stok sayısını 1 artırıyorum
            OleDbCommand stokArtir = new OleDbCommand(
                "UPDATE Kitaplar SET Adet = Adet + 1 WHERE KitapID = ?", conn);
            stokArtir.Parameters.Add("?", OleDbType.Integer).Value = kitapID;
            stokArtir.ExecuteNonQuery();
        }
    }
}
