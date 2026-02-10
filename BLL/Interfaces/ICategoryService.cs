using BLL.DTO;
using DAL;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        bool AddCategory(CategoryDTO categoryDTO);
        List<CategoryDTO> GetAllCategories();
    }
}
