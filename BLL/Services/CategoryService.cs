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
        private readonly IRepository<Category> repository;

        public CategoryService(IMapper mapper, IRepository<Category> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public bool AddCategory(CategoryDTO categoryDTO)
        {
            if (repository.Find(c => c.Name == categoryDTO.Name) != null)
                throw new ValidationException("Така категорія вже існує");

            var category = mapper.Map<Category>(categoryDTO);
            repository.Add(category);
            repository.SaveChanges();

            return true;
        }

        public List<CategoryDTO> GetAllCategories()
        {
            var categories = repository.GetAll();
            return mapper.Map<List<CategoryDTO>>(categories);
        }
    }
}