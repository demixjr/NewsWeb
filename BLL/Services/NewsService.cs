using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL;
using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace BLL.Services
{
    public class NewsService : INewsService
    {
        private readonly IMapper mapper;

        public NewsService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public bool AddNews(IRepository<News> repository, NewsDTO newsDTO)
        {
            var news = mapper.Map<News>(newsDTO);
            news.Date = DateTime.Now;
            news.Views = 0;

            repository.Add(news);
            return true;
        }

        public bool EditNews(IRepository<News> repository, NewsDTO newsDTO, int currentUserId)
        {
            var news = repository.Find(n => n.Id == newsDTO.Id);

            if (news == null)
                throw new Exception("Новину не знайдено");

            if (news.AuthorId != currentUserId)
                throw new ValidationException("Редагувати може тільки автор");

            news.Title = newsDTO.Title;
            news.Description = newsDTO.Description;
            news.CategoryId = newsDTO.CategoryId;

            repository.Update(news);
            return true;
        }

        public bool DeleteNews(IRepository<News> repository, int newsId, int currentUserId)
        {
            var news = repository.Find(n => n.Id == newsId);
            if (news == null)
                throw new Exception("Новину не знайдено");

            if (news.AuthorId != currentUserId)
                throw new ValidationException("Видаляти може тільки автор");

            repository.Remove(news);
            return true;
        }

        public NewsDTO? GetById(IRepository<News> repository, int id)
        {
            var news = repository.Find(n => n.Id == id);
            if (news == null)
                return null;

            news.Views++;
            repository.Update(news);
            return mapper.Map<NewsDTO>(news);
        }

        public List<NewsDTO> GetAll(IRepository<News> repository)
        {
            return mapper.Map<List<NewsDTO>>(repository.GetAll());
        }

        public List<NewsDTO> GetByCategory(
            IRepository<News> repository,
            int categoryId)
        {
            var news = repository
                .GetAll()
                .Where(n => n.CategoryId == categoryId);

            return mapper.Map<List<NewsDTO>>(news);
        }

        public List<NewsDTO> GetSortedByDate(IRepository<News> repository, bool descending = true)
        {
            var news = descending
                ? repository.GetAll().OrderByDescending(n => n.Date)
                : repository.GetAll().OrderBy(n => n.Date);

            return mapper.Map<List<NewsDTO>>(news);
        }

        public List<NewsDTO> GetPopular(IRepository<News> repository, int minViews)
        {
            var news = repository
                .GetAll()
                .Where(n => n.Views >= minViews)
                .OrderByDescending(n => n.Views);

            return mapper.Map<List<NewsDTO>>(news);
        }
    }
}