namespace SsoAuth
{
    public interface IUserService 
    { 
      public User? GetUserByEmail(string email);
    }

    public class UserService : IUserService
    {
        private readonly IEnumerable<User> _users;

        public UserService()
        {
            _users = new List<User>()
            {
               new User("Nafis", "nafis0014@gmail.com", "Admin", true),
               new User("Sajid", "sajid123@gmail.com", "User", true)
            };
        }

        public User? GetUserByEmail(string email) =>
          _users.FirstOrDefault(x => x.Email.ToLower() ==  email.ToLower());
    }

    public record User(
      string Name,
      string Email,
      string Role,
      bool Active
    );
}
