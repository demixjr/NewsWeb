using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddUser(UserDTO userDTO);
        Task<UserDTO?> FindUserByUsername(string? username);
        Task<bool> DeleteUser(UserDTO userDTO);
    }
}