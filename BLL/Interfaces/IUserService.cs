using BLL.DTO;
using DAL;
using DAL.Models;
namespace BLL.Interfaces
{
    public interface IUserService
    {
        bool AddUser(UserDTO userDTO);
        UserDTO FindUserByUsername(string username);
        bool DeleteUser(UserDTO userDTO);
    }
}
