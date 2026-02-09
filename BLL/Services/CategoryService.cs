using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL;
using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper mapper;

        public CategoryService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public bool AddCategory(IRepository<Category> repository, CategoryDTO categoryDTO)
        {
            if (repository.Find(c => c.Name == categoryDTO.Name) != null)
                throw new ValidationException("Така категорія вже існує");

            var category = mapper.Map<Category>(categoryDTO);
            repository.Add(category);

            return true;
        }

        public List<CategoryDTO> GetAllCategories(IRepository<Category> repository)
        {
            var categories = repository.GetAll();
            return mapper.Map<List<CategoryDTO>>(categories);
        }
    }
}