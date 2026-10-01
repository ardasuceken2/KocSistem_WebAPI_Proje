using Microsoft.EntityFrameworkCore;
using TaskApi.Domain;
using TaskApi.Infrastructure;
using Xunit;

namespace TaskApi.Tests
{
    public class TaskItemTests
    {
        [Fact] 
        public void AddTask_ShouldSaveToDatabase_Successfully()
        {
            
  
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_GörevEkleme")
                .Options;

            using var context = new AppDbContext(options);

  
            var fakeTask = new TaskItem
            {
                Title = "Unit Test Görevi",
                Description = "Bu bir testtir.",
                IsCompleted = false
            };

           
            context.TaskItems.Add(fakeTask);
            context.SaveChanges();

            var savedTask = context.TaskItems.FirstOrDefault(t => t.Title == "Unit Test Görevi");

            Assert.NotNull(savedTask); // veri boþ olamaz
            Assert.Equal("Bu bir testtir.", savedTask.Description); // açýklama doðru mu
            Assert.False(savedTask.IsCompleted); // false olmalý
        }
    }
}