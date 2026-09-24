using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IlkApim.DTOs
{
    /// <summary>
    /// Sisteme yeni görev eklerken kullanıcıdan alınacak verileri kısıtlayan model. 
    /// </summary>
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "Boş isim olamaz.")]
        [MaxLength(10, ErrorMessage = "10 karakterden fazla liste adı olamaz.")]
        [DefaultValue("Yazi yaz")]
        public string NewTask { get; set; } = string.Empty;
    }
}