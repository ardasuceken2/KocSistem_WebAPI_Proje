using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApi.Application;
using TaskApi.Domain;
using TaskApi.Infrastructure;
using System.Linq;

namespace TaskApi.Controllers
{
    [Authorize] // İçerideki tüm metotlar giriş yapmayı zorunlu kılar
    [ApiController]
    [Route("api/[controller]")] // 'api/tasks' olarak dışarı açılır
    public class TasksController : ControllerBase
    {
        // 1. Veritabanı bağlantımızı tutacağımız değişken
        private readonly AppDbContext _context;

        // 2. Veritabanını Dependency Injection ile içeri alıyoruz
        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        #region API Uç Noktaları (Endpoints)

        /// <summary>   
        /// Sistemdeki tüm görevleri listeler. Giriş yapmış userlar erişebilir.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            // Artık geçici listeyi değil, gerçek SQL tablosunu çekiyoruz
            var tasks = _context.TaskItems.ToList();
            return Ok(tasks);
        }

        /// <summary>
        /// Admin yeni görevler ekleyebilir yetkisi verildi.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add([FromBody] CreateTaskDto request)
        {
            // DTO'dan gelen veriyi (kuryeyi), SQL tablomuza (TaskItem) dönüştürüyoruz
            var newTask = new TaskItem
            {
                Title = request.NewTask,
                Description = "Kullanıcı tarafından eklendi.", // Varsayılan açıklama
                IsCompleted = false
            };

            _context.TaskItems.Add(newTask);
            _context.SaveChanges(); // Ve SQL'e kalıcı olarak kaydet!

            return Ok("Görev veritabanına eklendi: " + request.NewTask);
        }

        /// <summary>
        /// ID numarasına göre görevi siler. Sadece Admin erişebilir.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) // İndeks yerine artık veritabanı ID'si kullanıyoruz
        {
            var taskDb = _context.TaskItems.Find(id); // SQL'de bu ID'yi ara

            if (taskDb == null)
            {
                return NotFound("Veritabanında böyle bir ID bulunamadı.");
            }

            _context.TaskItems.Remove(taskDb);
            _context.SaveChanges(); // SQL'den kalıcı olarak sil

            return Ok("Görev Silindi: " + taskDb.Title);
        }

        /// <summary>
        /// ID numarasına göre görevi günceller. Sadece Admin erişebilir.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] string newTitle)
        {
            var taskDb = _context.TaskItems.Find(id); // SQL'de bu ID'yi ara

            if (taskDb == null)
            {
                return NotFound("Veritabanında böyle bir ID bulunamadı.");
            }

            taskDb.Title = newTitle; // İsmi değiştir
            _context.SaveChanges(); // SQL'de güncelle

            return Ok("Veri güncellendi: " + newTitle);
        }

        #endregion
    }
}