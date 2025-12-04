using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Category
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(15)]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public byte[]? Picture { get; set; }
    }
}
