using System;

namespace TaskApi.Domain
{
    /// <summary">
    /// Sisteme giriş yapacak kullanıcıyı ve güvenlik (kilitlenme) özelliklerini temsil eder.
    /// </summary>
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // Brute-force koruması için özellikler
        public bool IsLockedOut { get; set; }
        public DateTime? LockoutTime { get; set; }
        public int FailedAttemptCount { get; set; }
    }
}