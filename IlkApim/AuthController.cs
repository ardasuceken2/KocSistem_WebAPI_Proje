using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IlkApim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult KayitOl([FromBody] KullaniciLoginDto gelenForm)
        {
            
            var varOlanKullanici = DenemeVeriTabani.Kullanicilar
                .FirstOrDefault(k => k.KullaniciAdi == gelenForm.KullaniciAdi);

            if (varOlanKullanici != null)
            {
                _logger.LogWarning("Kayıt başarısız: {KullaniciAdi} zaten sistemde kayıtlı.", gelenForm.KullaniciAdi);
                return BadRequest("Bu kullanıcı adı zaten sistemde kayıtlı!");
            }

            var yeniKullanici = new Kullanici
            {
                KullaniciAdi = gelenForm.KullaniciAdi,
                Sifre = gelenForm.Sifre,
                Rol = "User"
            };

           
            DenemeVeriTabani.Kullanicilar.Add(yeniKullanici);
            _logger.LogInformation("Yeni kullanıcı kaydedildi: {KullaniciAdi}", gelenForm.KullaniciAdi);

            return Ok(new { Mesaj = "Kayıt başarıyla oluşturuldu! Artık login olabilirsiniz." });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] KullaniciLoginDto gelenForm)
        {
         
            var kullanici = DenemeVeriTabani.Kullanicilar
                .FirstOrDefault(k => k.KullaniciAdi == gelenForm.KullaniciAdi);

            if (kullanici == null)
            {
                _logger.LogWarning("Giriş başarısız (Kullanıcı Bulunamadı): {KullaniciAdi}", gelenForm.KullaniciAdi);
                return Unauthorized("Kullanıcı adı hatalı");
            }

            if (kullanici.KilitliMi)
            {
                if (kullanici.KilitlenmeZamani.HasValue && (DateTime.Now - kullanici.KilitlenmeZamani.Value).TotalSeconds < 15)
                {
                    var kalanSure = 15 - (int)(DateTime.Now - kullanici.KilitlenmeZamani.Value).TotalSeconds;
                    _logger.LogWarning("Timeout aktif. {KullaniciAdi} için kalan süre: {Kalan} sn", gelenForm.KullaniciAdi, kalanSure);
                    return BadRequest($"Çok fazla hatalı şifre girdiniz. Lütfen {kalanSure} saniye bekleyin.");
                }
                else
                {
                    kullanici.KilitliMi = false;
                    kullanici.HataliDenemeSayisi = 0;
                    kullanici.KilitlenmeZamani = null;
                    _logger.LogInformation("{KullaniciAdi} adlı kullanıcının 15 saniyelik kilidi kalktı.", gelenForm.KullaniciAdi);
                }
            }

            if (kullanici.Sifre != gelenForm.Sifre)
            {
                kullanici.HataliDenemeSayisi++;
                _logger.LogWarning("Hatalı şifre. Kullanıcı: {KullaniciAdi}, Deneme: {Sayi}/3", gelenForm.KullaniciAdi, kullanici.HataliDenemeSayisi);

                if (kullanici.HataliDenemeSayisi >= 3)
                {
                    kullanici.KilitliMi = true;
                    kullanici.KilitlenmeZamani = DateTime.Now;
                    _logger.LogError("GÜVENLİK: {KullaniciAdi} 3 kez hatalı girdi, 15 sn kilitlendi!", gelenForm.KullaniciAdi);
                    return BadRequest("3 kez hatalı şifre girdiğiniz için hesabınız 15 saniye kilitlenmiştir.");
                }

                return Unauthorized($"Şifre hatalı. Kalan deneme hakkınız: {3 - kullanici.HataliDenemeSayisi}");
            }

            kullanici.HataliDenemeSayisi = 0;
            kullanici.KilitliMi = false;
            kullanici.KilitlenmeZamani = null;

            var kullaniciHaklari = new[]
            {
                new Claim(ClaimTypes.Name, kullanici.KullaniciAdi),
                new Claim(ClaimTypes.Role, kullanici.Rol)
            };

            var jwtSecret = _configuration["JwtSettings:SecretKey"];
            var gizliAnahtar = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var imza = new SigningCredentials(gizliAnahtar, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(claims: kullaniciHaklari, expires: DateTime.Now.AddHours(1), signingCredentials: imza);
            var uretilenToken = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Başarılı giriş yapıldı: {KullaniciAdi} (Rol: {Rol})", kullanici.KullaniciAdi, kullanici.Rol);
            return Ok(new { Mesaj = "Giriş başarılı", Token = uretilenToken, Rütbe = kullanici.Rol });
        }
    }
}