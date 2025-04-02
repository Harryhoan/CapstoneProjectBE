namespace Application.ViewModels.CategoryDTO
{
    public class ViewCategory
    {
        public int CategoryId { get; set; }
        public int ParentCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
