using Microsoft.EntityFrameworkCore;
using TaskApi.Domain;
using TaskApi.Infrastructure;
using Xunit;

namespace TaskApi.Tests
{
    public class TaskItemTests
    {
        [Fact] // xUnit'e bunun bir "Test Metodu" olduðunu söyleriz
        public void AddTask_ShouldSaveToDatabase_Successfully()
        {
            
            // Gerçek SQL'e deðil, RAM'de "TestDb" adýnda sanal bir veritabanýna baðlanýyoruz
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_GörevEkleme")
                .Options;

            using var context = new AppDbContext(options);

            // Veritabanýna kaydedilecek sahte (fake) görevimizi hazýrlýyoruz
            var fakeTask = new TaskItem
            {
                Title = "Unit Test Görevi",
                Description = "Bu bir testtir.",
                IsCompleted = false
            };

           
            context.TaskItems.Add(fakeTask);
            context.SaveChanges();

            var savedTask = context.TaskItems.FirstOrDefault(t => t.Title == "Unit Test Görevi");

            Assert.NotNull(savedTask); // 1. Doðrulama: Kaydedilen veri null (boþ) OLMAMALI!
            Assert.Equal("Bu bir testtir.", savedTask.Description); // 2. Doðrulama: Açýklama doðru mu?
            Assert.False(savedTask.IsCompleted); // 3. Doðrulama: Görev tamamlanmamýþ (False) olmalý.
        }
    }
}