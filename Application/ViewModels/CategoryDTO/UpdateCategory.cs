namespace Application.ViewModels.CategoryDTO
{
    public class UpdateCategory
    {
        public int ParentCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
