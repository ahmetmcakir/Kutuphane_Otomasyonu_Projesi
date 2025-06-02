using System;
using System.IO;

namespace KutuphaneOtomasyon.DAL
{
    public static class LogHelper
    {
        // Log dosyasının konumu: Uygulama klasöründe /Logs/hata_kayitlari.txt
        private static readonly string logKlasoru = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly string logDosyaYolu = Path.Combine(logKlasoru, "hata_kayitlari.txt");

        /// <summary>
        /// Hata mesajını log dosyasına yazar. Dosya yoksa oluşturur.
        /// </summary>
        /// <param name="hataMesaji">Kaydedilecek hata mesajı</param>
        public static void LogYaz(string hataMesaji)
        {
            try
            {
                // Log klasörü yoksa oluştur
                if (!Directory.Exists(logKlasoru))
                {
                    Directory.CreateDirectory(logKlasoru);
                }

                // Log içeriği
                string logIcerik = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Hata:\n{hataMesaji}\n";

                // Dosyaya ekle
                File.AppendAllText(logDosyaYolu, logIcerik);
            }
            catch
            {
                // Loglama başarısızsa sessiz geç (sonsuz döngü oluşmaması için)
            }
        }
    }
}
