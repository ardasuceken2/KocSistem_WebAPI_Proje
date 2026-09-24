using System.ComponentModel.DataAnnotations;

namespace IlkApim.DTOs
{
    /// <summary>
    /// Kullanıcı giriş ve kayıt işlemlerinde gerekli verileri taşıyan ve doğrulayan model.
    /// </summary>
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Kullanıcı adı boş olamaz.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre boş olamaz.")]
        public string Password { get; set; } = string.Empty;
    }
}