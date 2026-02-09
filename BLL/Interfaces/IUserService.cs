using BLL.DTO;
using DAL;
using DAL.Models;
namespace BLL.Interfaces
{
    public interface IUserService
    {
        bool AddUser(IRepository<User> repository, UserDTO userDTO);
        UserDTO FindUserByUsername(IRepository<User> repository, string username);
        bool DeleteUser(IRepository<User> repository, UserDTO userDTO);
    }
}
