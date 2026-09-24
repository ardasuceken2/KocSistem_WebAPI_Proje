namespace IlkApim
{
    public static class DenemeVeriTabani
    {

        public static List<Kullanici> Kullanicilar { get; set; } = new List<Kullanici>
        {
            new Kullanici { KullaniciAdi = "Admin", Sifre = "123456", Rol = "Admin" }
        };
    }
}