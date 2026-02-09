using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL;
using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        IMapper mapper;
        public UserService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public bool AddUser(IRepository<User> repository, UserDTO userDTO)
        {
            if (FindUserByUsername(repository, userDTO.Username) != null)
                throw new ValidationException("Такий користувач вже існує");

            var user = mapper.Map<User>(userDTO);
            repository.Add(user);
            return true;
        }

        public UserDTO FindUserByUsername(IRepository<User> repository, string? username)
        {
            return mapper.Map<UserDTO>(repository.Find(c => c.Username == username));
        }
        public bool DeleteUser(IRepository<User> repository, UserDTO userDTO)
        {

            var user = repository.Find(u => u.Username == userDTO.Username);
            if (user == null)
                throw new Exception("Неможливо видалити користувача, оскільки його не знайдено в базі даних");

            repository.Remove(user);
            return true;
        }
    }
}
