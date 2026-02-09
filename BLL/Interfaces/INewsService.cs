using BLL.DTO;
using DAL;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface INewsService
    {
        bool AddNews(IRepository<News> repository, NewsDTO newsDTO);

        bool EditNews( IRepository<News> repository, NewsDTO newsDTO, int currentUserId);

        bool DeleteNews(IRepository<News> repository, int newsId, int currentUserId);
        NewsDTO? GetById(IRepository<News> repository, int id);
        List<NewsDTO> GetAll(IRepository<News> repository);

        List<NewsDTO> GetByCategory(IRepository<News> repository,int categoryId);

        List<NewsDTO> GetSortedByDate(IRepository<News> repository, bool descending = true);

        List<NewsDTO> GetPopular(IRepository<News> repository, int minViews);
    }
}