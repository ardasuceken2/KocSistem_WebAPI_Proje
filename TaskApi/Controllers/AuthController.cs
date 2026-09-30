using System;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
// Kendi katmanlarımız:
using TaskApi.Domain;
using TaskApi.Application;
using TaskApi.Infrastructure;

namespace TaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Bağımlılıklar ve Yapıcı Metot
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        // 1. AppDbContext Eklendi (SQL bağlantısı için)
        private readonly AppDbContext _context; 

        // 2. Yapıcı metotta AppDbContext istendi
        public AuthController(ILogger<AuthController> logger, IConfiguration configuration, AppDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }
        #endregion

        #region API Uç Noktaları (Endpoints)

        /// <summary>
        /// Sisteme yeni bir kullanıcı kaydeder.
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserLoginDto request)
        {
            // 3. Database.Users YERİNE _context.Users KULLANILIYOR
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Username == request.Username);

            if (existingUser != null)
            {
                _logger.LogWarning("Kayıt başarısız: {Username} zaten sistemde kayıtlı.", request.Username);
                return BadRequest("Bu kullanıcı adı zaten sistemde kayıtlı!");
            }

            var newUser = new User
            {
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password), // ŞİFREYİ HASHLEYEREK KAYDET
                Role = "User"
            };

            // 4. Yeni kullanıcıyı SQL'e ekle ve KAYDET
            _context.Users.Add(newUser);
            _context.SaveChanges(); 

            _logger.LogInformation("Yeni kullanıcı kaydedildi: {Username}", request.Username);

            return Ok(new { Message = "Kayıt başarıyla oluşturuldu! Artık login olabilirsiniz." });
        }

        /// <summary>
        /// Kullanıcıyı doğrular, hatalı girişleri sayar ve JWT token döner.
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto request)
        {
            // 5. Giriş yapacak kullanıcıyı SQL'den ara
            var user = _context.Users
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
                    // Süre dolmuşsa kilidi aç ve SQL'i GÜNCELLE
                    user.IsLockedOut = false;
                    user.FailedAttemptCount = 0;
                    user.LockoutTime = null;
                    _context.SaveChanges(); 
                    _logger.LogInformation("{Username} adlı kullanıcının 15 saniyelik kilidi kalktı.", request.Username);
                }
            }

            // Şifre kontrolü (Hash karşılaştırması)
            bool isPasswordValid = false;
            try 
            {
                // SQL'deki şifre BCrypt formatındaysa doğrula, düz metinse direkt kontrol et
                isPasswordValid = user.Password.StartsWith("$2a$") || user.Password.StartsWith("$2b$") 
                                  ? BCrypt.Net.BCrypt.Verify(request.Password, user.Password)
                                  : user.Password == request.Password;
            }
            catch { }

            if (!isPasswordValid)
            {
                user.FailedAttemptCount++;
                _context.SaveChanges(); // Hatalı girişi SQL'e kaydet

                _logger.LogWarning("Hatalı şifre. Kullanıcı: {Username}, Deneme: {Count}/3", request.Username, user.FailedAttemptCount);

                if (user.FailedAttemptCount >= 3)
                {
                    user.IsLockedOut = true;
                    user.LockoutTime = DateTime.Now;
                    _context.SaveChanges(); // Kilitlenmeyi SQL'e kaydet
                    _logger.LogError("GÜVENLİK: {Username} 3 kez hatalı girdi, 15 sn kilitlendi!", request.Username);
                    return BadRequest("3 kez hatalı şifre girdiğiniz için hesabınız 15 saniye kilitlenmiştir.");
                }

                return Unauthorized($"Şifre hatalı. Kalan deneme hakkınız: {3 - user.FailedAttemptCount}");
            }

            // Başarılı giriş, kilitleri sıfırla ve SQL'e kaydet
            user.FailedAttemptCount = 0;
            user.IsLockedOut = false;
            user.LockoutTime = null;
            _context.SaveChanges(); 

            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var jwtSecret = _configuration["JwtSettings:SecretKey"];
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!));
            var signature = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(claims: userClaims, expires: DateTime.Now.AddHours(1), signingCredentials: signature);
            var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Başarılı giriş yapıldı: {Username} (Rol: {Role})", user.Username, user.Role);
            return Ok(new { Message = "Giriş başarılı", Token = generatedToken, Role = user.Role });
        }

        /// <summary>
        /// Sadece Admin'in görebileceği kullanıcı listesi
        /// </summary>
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            // Şifreleri gizleyerek kullanıcı listesini gönderiyoruz
            var users = _context.Users.Select(u => new { u.Id, u.Username, u.Role, u.IsLockedOut }).ToList();
            return Ok(users);
        }

        /// <summary>
        /// Sadece Admin'in kullanabileceği kullanıcı silme işlemi
        /// </summary>
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound(new { Message = "Kullanıcı bulunamadı." });
            }

            if (user.Role == "Admin")
            {
                return BadRequest(new { Message = "Admin yetkisine sahip kullanıcılar silinemez!" });
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            _logger.LogInformation("{UserId} ID'li kullanıcı Admin tarafından silindi.", id);
            return Ok(new { Message = "Kullanıcı başarıyla silindi." });
        }

        #endregion
    }
}