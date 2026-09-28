using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using TaskApi.Application;
using TaskApi.Domain;
using TaskApi.Infrastructure;

namespace TaskApi.Controllers
{
    [Authorize] // İçerideki tüm metotlar giriş yapmayı zorunlu kılar
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        #region API Uç Noktaları (Endpoints)

        /// <summary>   
        /// Sistemdeki görevleri listeler. Admin herkesi, User ise sadece kendini görür.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var username = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == "Admin")
            {
                // Admin herkesin görevini görebilir
                var allTasks = _context.TaskItems
                    .Include(t => t.AssignedUser) // Kullanıcı bilgisini de SQL'den çek
                    .Select(t => new {
                        t.Id, t.Title, t.Description, t.IsCompleted, t.CreatedDate,
                        AssignedUserName = t.AssignedUser != null ? t.AssignedUser.Username : "Atanmadı"
                    }).ToList();
                return Ok(allTasks);
            }
            else
            {
                // Normal kullanıcı SADECE kendisine atanan görevleri görebilir
                var myTasks = _context.TaskItems
                    .Include(t => t.AssignedUser)
                    .Where(t => t.AssignedUser != null && t.AssignedUser.Username == username)
                    .Select(t => new {
                        t.Id, t.Title, t.Description, t.IsCompleted, t.CreatedDate,
                        AssignedUserName = username
                    }).ToList();
                return Ok(myTasks);
            }
        }

        /// <summary>
        /// Admin yeni görev ekleyip, belli bir kişiye atayabilir.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add([FromBody] CreateTaskDto request)
        {
            var newTask = new TaskItem
            {
                Title = request.NewTask,
                Description = "Kullanıcıya özel görev.",
                IsCompleted = false,
                AssignedUserId = request.AssignedUserId // YENİ: Kime atanacak?
            };

            _context.TaskItems.Add(newTask);
            _context.SaveChanges(); 

            return Ok(new { Message = "Görev başarıyla eklendi ve atandı." });
        }

        /// <summary>
        /// ID numarasına göre görevi siler.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var taskDb = _context.TaskItems.Find(id);
            if (taskDb == null) return NotFound("Veritabanında böyle bir ID bulunamadı.");

            _context.TaskItems.Remove(taskDb);
            _context.SaveChanges();
            return Ok(new { Message = "Görev Silindi." });
        }

        /// <summary>
        /// ID numarasına göre görevin başlığını günceller.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] string newTitle)
        {
            var taskDb = _context.TaskItems.Find(id);
            if (taskDb == null) return NotFound("Veritabanında böyle bir ID bulunamadı.");

            taskDb.Title = newTitle;
            _context.SaveChanges();
            return Ok(new { Message = "Veri güncellendi." });
        }

        #endregion
    }
}