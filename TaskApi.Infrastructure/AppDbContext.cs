using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TaskApi.Domain;

namespace TaskApi.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // C#'taki TaskItem modelini, SQL'de TaskItems adında bir tabloya dönüştürür
        public DbSet<TaskItem> TaskItems { get; set; }

        // Kullanıcıları SQL'e kaydetmek için Users tablosunu ekliyoruz
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Veritabanı ilk oluştuğunda varsayılan Admin kullanıcısını SQL'e ekler
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "Admin", 
                    Password = "123456", 
                    Role = "Admin", 
                    FailedAttemptCount = 0, 
                    IsLockedOut = false 
                }
            );
        }
    }
}