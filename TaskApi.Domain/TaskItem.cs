using System;

namespace TaskApi.Domain
{
    /// <summary>
    /// Veritabanındaki TaskItems tablosunun karşılığı olan sınıf.
    /// </summary>
    public class TaskItem
    {
        public int Id { get; set; } // SQL'deki Primary Key  otomatik artan +1 ID
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}