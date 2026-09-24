namespace IlkApim
{
    public class Kullanici
    {
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string Rol { get; set; } = "User";


        public int HataliDenemeSayisi { get; set; } =0;
        public bool KilitliMi { get; set; } = false;
        public DateTime? KilitlenmeZamani { get; set; }
    }
}