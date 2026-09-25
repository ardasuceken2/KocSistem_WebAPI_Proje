    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TaskApi.Application;

    namespace TaskApi.Controllers
    {
        [Authorize]
        [ApiController]
        [Route("api/[controller]")] // 'api/tasks' olarak dışarı açılır
        public class TasksController : ControllerBase
        {
            // TODO: Bu geçici liste mimari değiştiğinde veritabanından çekilecek.
            private static List<string> _tasks = new List<string>
            {
                "C# çalış",
                "Git öğren"
            };

            #region API Uç Noktaları (Endpoints)

            /// <summary>   
            /// Sistemdeki tüm görevleri listeler. Giriş yapmış userlar erişebilir.
            /// </summary>
            [HttpGet]
            public IActionResult GetAll()
            {
                return Ok(_tasks);
            }

            /// <summary>
            /// Admin yeni görevler ekleyebilir yetkisi verildi.
            /// </summary>
            [Authorize(Roles = "Admin")]
            [HttpPost]
            public IActionResult Add([FromBody] CreateTaskDto request)
            {
                _tasks.Add(request.NewTask);
                return Ok("Görev eklendi: " + request.NewTask);
            }

            /// <summary>
            /// İndeks numarasına göre görevi siler. Sadece Admin erişebilir.
            /// </summary>
            [Authorize(Roles = "Admin")]
            [HttpDelete("{index}")]
            public IActionResult Delete(int index)
            {
                if (index < 0 || index >= _tasks.Count)
                {
                    return NotFound("Böyle bir task yok.");
                }

                string deletedTask = _tasks[index];
                _tasks.RemoveAt(index);

                return Ok("Silindi: " + deletedTask);
            }

            /// <summary>
            /// İndeks numarasına göre görevi günceller Sadece Admin erişebilir.
            /// </summary>
            [Authorize(Roles = "Admin")]
            [HttpPut("{index}")]
            public IActionResult Update(int index, [FromBody] string newTitle)
            {
                if (index < 0 || index >= _tasks.Count)
                {
                    return NotFound("Böyle bir task yok.");
                }

                _tasks[index] = newTitle;
                return Ok("Veri güncellendi: " + newTitle);
            }

            #endregion
        }
    }