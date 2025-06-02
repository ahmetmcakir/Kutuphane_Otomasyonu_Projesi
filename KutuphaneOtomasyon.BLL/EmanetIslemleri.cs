using System;
using System.Data;

namespace KutuphaneOtomasyon.BLL
{
    public class EmanetIslemleri
    {
        // Bu metot, DataTable'daki teslim edilmemiş kitaplara ait gecikme durumunu kontrol eder ve ceza hesaplar.
        public static DataTable CezaHesapla(DataTable dt)
        {
            // Gecikme başına alınacak ceza oranını belirledim.
            decimal cezaOrani = 1.0m;

            // Sistem tarihini alarak günümüz tarihini tanımladım.
            DateTime bugun = DateTime.Today;

            // Veritabanından gelen tüm emanet kayıtlarını tek tek kontrol ediyorum.
            foreach (DataRow row in dt.Rows)
            {
                // Sadece henüz teslim edilmemiş kitaplar için işlem yapıyorum.
                if (row["TeslimEdildiMi"].ToString() != "True")
                {
                    // İade tarihini tablo satırından alıp DateTime'a dönüştürdüm.
                    DateTime iadeTarihi = Convert.ToDateTime(row["IadeTarihi"]);

                    // Gecikme gün sayısını hesaplıyorum.
                    int gecikmeGun = (bugun - iadeTarihi).Days;

                    // Gecikme varsa ceza miktarını hesaplıyorum, yoksa ceza 0 TL.
                    decimal ceza = gecikmeGun > 0 ? gecikmeGun * cezaOrani : 0;

                    // Durum sütununa gecikme varsa gün ve ceza bilgisiyle birlikte "Geciken",
                    // gecikme yoksa "Devam Eden" olarak yazdırıyorum.
                    row["Durum"] = gecikmeGun > 0
                        ? $"Geciken ({gecikmeGun} gün, {ceza:F2} TL)"
                        : "Devam Eden";
                }
            }

            // Güncellenmiş tabloyu formda göstermek için geri döndürüyorum.
            return dt;
        }
    }
}
