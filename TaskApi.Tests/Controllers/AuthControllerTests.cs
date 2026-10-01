using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using TaskApi.Application;
using TaskApi.Controllers;
using TaskApi.Domain;
using TaskApi.Infrastructure;
using Xunit;
using System.Collections.Generic;

namespace TaskApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public void Register_YeniKullanici_BasariylaKayitOlmali()
        {

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthTestDb_Register")
                .Options;
            using var context = new AppDbContext(options);

            // B) Sahte IConfiguration (appsettings.json) kuruyoruz
            var inMemorySettings = new Dictionary<string, string?> {
                {"JwtSettings:SecretKey", "bizim_cok_gizli_test_sifremiz_1234567890_test_icin"}
            };
            IConfiguration fakeConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // C) Sahte Logger (Senin Controller'ın log tuttuğu için boş bir sahte loglayıcı veriyoruz)
            var fakeLogger = NullLogger<AuthController>.Instance;

            // D) Test edeceğimiz Controller'ı sahte verilerimizle ayağa kaldırıyoruz
            var controller = new AuthController(fakeLogger, fakeConfiguration, context);



            // ==========================================
            // 2. ACT (EYLEM)
            // ==========================================
            // React'ten geliyormuş gibi sahte bir kullanıcı çantası (DTO) hazırlıyoruz
            var newUserRequest = new UserLoginDto
            {
                Username = "test_stajyer",
                Password = "SuperGizliSifre123"
            };

            // Controller'ın Register metoduna bu isteği gönderiyoruz
            var result = controller.Register(newUserRequest);

            // ==========================================
            // 3. ASSERT (DOĞRULAMA)
            // ==========================================
            // A) Controller bize 200 OK (Başarılı) döndü mü?
            Assert.IsType<OkObjectResult>(result);

            // B) Gerçekten sanal veritabanına bu isimde biri kaydedilmiş mi?
            var savedUser = context.Users.FirstOrDefault(u => u.Username == "test_stajyer");

            Assert.NotNull(savedUser); // 1. Veritabanına kesin kaydedilmiş olmalı (null olmamalı)
            Assert.Equal("User", savedUser.Role); // 2. Otomatik olarak "User" rolü verilmiş mi?
            Assert.NotEqual("SuperGizliSifre123", savedUser.Password); // 3. Güvenlik: Şifresi düz metin olarak değil, Hash'lenerek (BCrypt) mi kaydedilmiş?


        }

        [Fact]
        public void Login_DogruSifre_BasariliGirisVeTokenDonmeli()
        {
            // 1. ARRANGE (HAZIRLIK)
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthTestDb_LoginSuccess") 
                .Options;
            using var context = new AppDbContext(options);

            // Adamın login olabilmesi için önce sanal DB'ye bir kullanıcı "KAYDETMEMİZ" lazım
            context.Users.Add(new User 
            { 
                Username = "test_kullanicisi", 
                Password = BCrypt.Net.BCrypt.HashPassword("HarikaSifre123"), 
                Role = "User",
                FailedAttemptCount = 0,
                IsLockedOut = false
            });
            context.SaveChanges();

            // Sahte Ayarlar 
            var inMemorySettings = new Dictionary<string, string?> {
                {"JwtSettings:SecretKey", "bizim_cok_gizli_test_sifremiz_1234567890_test_icin"}
            };
            IConfiguration fakeConfig = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            var fakeLogger = NullLogger<AuthController>.Instance;
            
            var controller = new AuthController(fakeLogger, fakeConfig, context);

            // ==========================================
            // 2. ACT (EYLEM)
            // ==========================================
            var loginRequest = new UserLoginDto 
            { 
                Username = "test_kullanicisi", 
                Password = "HarikaSifre123" // Doğru şifreyi giriyoruz
            };
            
            var result = controller.Login(loginRequest);

            // ==========================================
            // 3. ASSERT (DOĞRULAMA)
            // ==========================================
            var okResult = Assert.IsType<OkObjectResult>(result); // Sistemin bize 200 OK döndüğünden emin oluyoruz
            Assert.NotNull(okResult.Value); // Sistemin bize boş değil, içinde "Token" olan bir cevap döndüğünden emin oluyoruz
        }

        [Fact]
        public void Login_YanlisSifre_YetkisizDonmeli_Ve_HataSayisiArtmali()
        {
            // 1. ARRANGE (HAZIRLIK)
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthTestDb_LoginFail") 
                .Options;
            using var context = new AppDbContext(options);

            // Sanal DB'ye şifresi HarikaSifre123 olan bir adam ekliyoruz
            context.Users.Add(new User 
            { 
                Username = "test_kullanicisi", 
                Password = BCrypt.Net.BCrypt.HashPassword("HarikaSifre123"), 
                Role = "User",
                FailedAttemptCount = 0,
                IsLockedOut = false
            });
            context.SaveChanges();

            var inMemorySettings = new Dictionary<string, string?> {
                {"JwtSettings:SecretKey", "bizim_cok_gizli_test_sifremiz_1234567890_test_icin"}
            };
            IConfiguration fakeConfig = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            var fakeLogger = NullLogger<AuthController>.Instance;
            
            var controller = new AuthController(fakeLogger, fakeConfig, context);

            // ==========================================
            // 2. ACT (EYLEM) - YANLIŞ ŞİFRE
            // ==========================================
            var loginRequest = new UserLoginDto 
            { 
                Username = "test_kullanicisi", 
                Password = "YanlisSifre_999" 
            };
            
            var result = controller.Login(loginRequest);

            // ==========================================
            // 3. ASSERT (DOĞRULAMA)
            // ==========================================
            // A) 401 Unauthorized dönmeli
            Assert.IsType<UnauthorizedObjectResult>(result); 

            // B) Veritabanındaki FailedAttemptCount (Hatalı Giriş Sayısı) tam olarak 1 artmalı
            var userInDb = context.Users.FirstOrDefault(u => u.Username == "test_kullanicisi");
            Assert.NotNull(userInDb);
            Assert.Equal(1, userInDb.FailedAttemptCount);
        }
    }
}