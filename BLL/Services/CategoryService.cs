using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL;
using DAL.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Category> _repository;

        public CategoryService(IMapper mapper, IRepository<Category> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<bool> AddCategory(CategoryDTO categoryDTO)
        {
            var exists = await _repository.GetAll()
                .AnyAsync(c => c.Name == categoryDTO.Name);

            if (exists)
                throw new ValidationException("Така категорія вже існує");

            var category = _mapper.Map<Category>(categoryDTO);
            await _repository.Add(category);
            await _repository.SaveChanges();

            return true;
        }

        public async Task<List<CategoryDTO>> GetAllCategories()
        {
            var categories = await _repository.GetAll().Include(c => c.News).ToListAsync();

            return _mapper.Map<List<CategoryDTO>>(categories);
        }
    }
}