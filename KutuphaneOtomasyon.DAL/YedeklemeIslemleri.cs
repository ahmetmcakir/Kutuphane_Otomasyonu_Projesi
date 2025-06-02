using System;
using System.IO;

namespace KutuphaneOtomasyon.DAL
{
    public static class YedeklemeIslemleri
    {
        public static string VeritabaniYedekle(string kaynakDosyaYolu, string hedefKlasor)
        {
            try
            {
                if (!Directory.Exists(hedefKlasor))
                    Directory.CreateDirectory(hedefKlasor);

                string yedekAdi = $"Kutuphane_{DateTime.Now:yyyyMMdd_HHmmss}.accdb";
                string hedefYol = Path.Combine(hedefKlasor, yedekAdi);

                File.Copy(kaynakDosyaYolu, hedefYol, overwrite: true);

                return hedefYol; // UI'de mesaj göstermek için geri döner
            }
            catch (Exception ex)
            {
                LogHelper.LogYaz(ex.ToString());
                throw; // Hata UI katmanına fırlatılır, orada gösterilir
            }
        }
    }
}
