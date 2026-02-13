using BLL.DTO;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<bool> AddCategory(CategoryDTO categoryDTO);
        Task<List<CategoryDTO>> GetAllCategories();
    }
}