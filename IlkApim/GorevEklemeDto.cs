using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IlkApim 
{
    public class GorevEkleDto
    {

        [Required(ErrorMessage = "boş isim olamaz")]
        [MaxLength(10, ErrorMessage = "10 karakterden fazla liste adı olamaz.")]
        [DefaultValue("Yazi yaz")]
        public string YeniGorev { get; set; } = String.Empty; 

    }



}