using SportsLend.BLL.Repository;
using SportsLend.DAL.Models;

namespace SportsLend.BLL.Service
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public UserService()
        {
            _userRepository = new UserRepository();
        }

        public async Task<User?> GetUserAccountAsync(string email, string password)
        {
            return await _userRepository.ValidateUserAsync(email, password);
        }
    }
}
