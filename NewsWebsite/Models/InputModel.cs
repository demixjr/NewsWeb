using System.ComponentModel.DataAnnotations;

namespace PL.Models
{
    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Обов'язкове поле")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Обов'язкове поле")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Оберіть категорію")]
        public int CategoryId { get; set; }
    }
}
