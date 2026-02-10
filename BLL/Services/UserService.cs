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
        IRepository<User> repository;

        public UserService(IMapper mapper, IRepository<User> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public bool AddUser(UserDTO userDTO)
        {
            if (FindUserByUsername(userDTO.Username) != null)
                throw new ValidationException("Такий користувач вже існує");

            var user = mapper.Map<User>(userDTO);
            repository.Add(user);
            repository.SaveChanges();

            return true;
        }

        public UserDTO FindUserByUsername(string? username)
        {
            return mapper.Map<UserDTO>(repository.Find(c => c.Username == username));
        }
        public bool DeleteUser(UserDTO userDTO)
        {

            var user = repository.Find(u => u.Username == userDTO.Username);
            if (user == null)
                throw new Exception("Неможливо видалити користувача, оскільки його не знайдено в базі даних");

            repository.Remove(user);
            repository.SaveChanges();

            return true;
        }
    }
}
