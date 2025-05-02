using UserEntityWebAPI.Components;

namespace UserEntityWebAPI.Services
{
    public class UserService
    {
        private readonly Dictionary<string, User> _users = new();   // Логин -> Пользователь
    }
}
