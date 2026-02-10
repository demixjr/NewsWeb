using BLL.DTO;
using DAL;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface INewsService
    {
        bool AddNews(NewsDTO newsDTO);

        bool EditNews(NewsDTO newsDTO, int currentUserId);

        bool DeleteNews(int newsId, int currentUserId);
        NewsDTO? GetById(int id);
        List<NewsDTO> GetAll();

        List<NewsDTO> GetByCategory(int categoryId);

        List<NewsDTO> GetSortedByDate(bool descending = true);

        List<NewsDTO> GetPopular(int minViews);
    }
}