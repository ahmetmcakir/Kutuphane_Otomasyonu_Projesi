using System;

namespace KutuphaneOtomasyon.DAL
{
    public class Uye
    {
        // Üyenin ID'si
        public int UyeID { get; set; }
        // Üyenin adı
        public string Ad { get; set; }
        // Üyenin soyadı
        public string Soyad { get; set; }
        // Girişte kullanılacak kullanıcı adı
        public string KullaniciAdi { get; set; }
        // Şifre
        public string Sifre { get; set; }
        // Telefon numarası
        public string Telefon { get; set; }
        // E-posta adresi
        public string Email { get; set; }
        // Kayıt tarihi
        public DateTime KayitTarihi { get; set; }

        public string Salt { get; set; }
        public string Yetki { get; set; }

    }
}