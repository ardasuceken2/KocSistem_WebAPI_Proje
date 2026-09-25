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
    }
}