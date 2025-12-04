namespace Application.Dtos.Category
{
    public class ReadCategoryDto
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public byte[]? Picture { get; set; }
    }
}
