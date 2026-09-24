using System.ComponentModel.DataAnnotations;

namespace IlkApim
{
    public class KullaniciLoginDto
    {
        [Required(ErrorMessage = "kullanici adi bos olamaz")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "sifre bos olamaz")]
        public string Sifre { get; set; } = string.Empty;
    }
}