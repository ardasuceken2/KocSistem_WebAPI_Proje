using IlkApim.Data;
using IlkApim.DTOs;
using IlkApim.Models;
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
        #region Bağımlılıklar ve Yapıcı Metot
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        #region API Uç Noktaları (Endpoints)

        /// <summary>
        /// Sisteme yeni bir kullanıcı kaydeder.
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserLoginDto request)
        {
            var existingUser = Database.Users
                .FirstOrDefault(u => u.Username == request.Username);

            if (existingUser != null)
            {
                _logger.LogWarning("Kayıt başarısız: {Username} zaten sistemde kayıtlı.", request.Username);
                return BadRequest("Bu kullanıcı adı zaten sistemde kayıtlı!");
            }

            var newUser = new User
            {
                Username = request.Username,
                Password = request.Password,
                Role = "User"
            };

            Database.Users.Add(newUser);
            _logger.LogInformation("Yeni kullanıcı kaydedildi: {Username}", request.Username);

            return Ok(new { Message = "Kayıt başarıyla oluşturuldu! Artık login olabilirsiniz." });
        }

        /// <summary>
        /// Kullanıcıyı doğrular, hatalı girişleri sayar ve JWT token döner.
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto request)
        {
            var user = Database.Users
                .FirstOrDefault(u => u.Username == request.Username);

            if (user == null)
            {
                _logger.LogWarning("Giriş başarısız (Kullanıcı Bulunamadı): {Username}", request.Username);
                return Unauthorized("Kullanıcı adı hatalı.");
            }

            // Kilit kontrolü
            if (user.IsLockedOut)
            {
                if (user.LockoutTime.HasValue && (DateTime.Now - user.LockoutTime.Value).TotalSeconds < 15)
                {
                    var remainingTime = 15 - (int)(DateTime.Now - user.LockoutTime.Value).TotalSeconds;
                    _logger.LogWarning("Timeout aktif. {Username} için kalan süre: {Remaining} sn", request.Username, remainingTime);
                    return BadRequest($"Çok fazla hatalı şifre girdiniz. Lütfen {remainingTime} saniye bekleyin.");
                }
                else
                {
                    // Süre dolmuşsa kilidi aç
                    user.IsLockedOut = false;
                    user.FailedAttemptCount = 0;
                    user.LockoutTime = null;
                    _logger.LogInformation("{Username} adlı kullanıcının 15 saniyelik kilidi kalktı.", request.Username);
                }
            }

            // Şifre kontrolü
            if (user.Password != request.Password)
            {
                user.FailedAttemptCount++;
                _logger.LogWarning("Hatalı şifre. Kullanıcı: {Username}, Deneme: {Count}/3", request.Username, user.FailedAttemptCount);

                if (user.FailedAttemptCount >= 3)
                {
                    user.IsLockedOut = true;
                    user.LockoutTime = DateTime.Now;
                    _logger.LogError("GÜVENLİK: {Username} 3 kez hatalı girdi, 15 sn kilitlendi!", request.Username);
                    return BadRequest("3 kez hatalı şifre girdiğiniz için hesabınız 15 saniye kilitlenmiştir.");
                }

                return Unauthorized($"Şifre hatalı. Kalan deneme hakkınız: {3 - user.FailedAttemptCount}");
            }

            // Başarılı giriş, kilitleri sıfırla
            user.FailedAttemptCount = 0;
            user.IsLockedOut = false;
            user.LockoutTime = null;

            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var jwtSecret = _configuration["JwtSettings:SecretKey"];
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var signature = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(claims: userClaims, expires: DateTime.Now.AddHours(1), signingCredentials: signature);
            var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Başarılı giriş yapıldı: {Username} (Rol: {Role})", user.Username, user.Role);
            return Ok(new { Message = "Giriş başarılı", Token = generatedToken, Role = user.Role });
        }

        #endregion
    }
}