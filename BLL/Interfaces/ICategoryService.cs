using BLL.DTO;
using DAL;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        bool AddCategory(IRepository<Category> repository, CategoryDTO categoryDTO);
        List<CategoryDTO> GetAllCategories(IRepository<Category> repository);
    }
}
