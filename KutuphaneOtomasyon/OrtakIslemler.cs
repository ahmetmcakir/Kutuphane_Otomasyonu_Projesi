using System.Windows.Forms;

namespace KutuphaneOtomasyon
{
    public static class OrtakIslemler
    {
        public static void Cikis(Form mevcutForm)
        {
            Form1 form1 = new Form1();
            form1.Show();
            mevcutForm.Hide();
        }
    }
}
