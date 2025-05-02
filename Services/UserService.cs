using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using UserEntityWebAPI.Models;

namespace UserEntityWebAPI.Services
{
    public class UserService
    {
        private readonly Dictionary<string, User> _users = new();   // Логин -> Пользователь

        /// <summary>
        /// Конструктор создания сервиса. При создании сервиса админ добавляется автоматически.
        /// </summary>
        public UserService() 
        {
            User admin = User.CreateAdmin();
            _users[admin.Login] = admin;
        }

        public User CreateUser(string login, string password, string name, int gender, DateTime? birthday, bool isAdmin, string createdBy)
        {
            if (!IsLatinLettersAndDigitsOnly(login))
                throw new InvalidDataException("Недопустимый логин");

            if (!IsLatinLettersAndDigitsOnly(password))
                throw new InvalidDataException("Недопустимый пароль");

            if (!IsLatinAndCyrillicLettersOnly(name))
                throw new InvalidDataException("Недопустимое имя");

            if (_users.ContainsKey(login))
                throw new InvalidOperationException("Логин уже занят");

            if (gender < 0 || gender > 2)
                throw new InvalidDataException("Недопустимое значение поля \"Пол\"");

            User user = new User(login, password, name, gender, birthday, isAdmin, createdBy);
            _users[login] = user;
            return user;
        }

        public void UpdatePassword(string login, string newPassword, string modifitedBy)
        {
            // TODO: Проверка на админа или самого пользователя

            if (!IsLatinLettersAndDigitsOnly(newPassword))
                throw new InvalidDataException("Недопустимый пароль");

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            user.UpdatePassword(newPassword, modifitedBy);
        }

        public void UpdateLogin(string login, string newLogin, string modifitedBy)
        {
            // TODO: Проверка на админа или самого пользователя

            if (!IsLatinLettersAndDigitsOnly(newLogin))
                throw new InvalidDataException("Недопустимый логин");

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            user.UpdateLogin(newLogin, modifitedBy);
        }

        public void UpdateName(string login, string newName, string modifitedBy)
        {
            // TODO: Проверка на админа или самого пользователя

            if (!IsLatinAndCyrillicLettersOnly(newName))
                throw new InvalidDataException("Недопустимое имя");

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            user.UpdateName(newName, modifitedBy);
        }

        public void UpdateGender(string login, int newGender, string modifitedBy)
        {
            // TODO: Проверка на админа или самого пользователя

            if (newGender < 0 || newGender > 2)
                throw new InvalidDataException("Недопустимое значение поля \"Пол\"");

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            user.UpdateGender(newGender, modifitedBy);
        }

        public void UpdateBirthday(string login, DateTime newBirthday, string modifitedBy)
        {
            // TODO: Проверка на админа или самого пользователя

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            user.UpdateBirthday(newBirthday, modifitedBy);
        }

        public IEnumerable<User> GetAllActiveUsers()
        {
            // TODO: Проверка на админа

            return _users.Values.Where(user => user.IsActive).OrderBy(user => user.CreatedOn);
        }

        public User? GetByLogin(string login)
        {
            // TODO: Проверка на админа

            if (_users.TryGetValue(login, out var user))
            {
                return user;
            }

            return null;
        }

        public User? GetMyUser(string login, string password)
        {
            if (_users.TryGetValue(login, out var user))
            {
                if (user.Password == password)
                    return user;
            }

            return null;
        }

        public IEnumerable<User> GetAllUsersOlderThan(int age)
        {
            // TODO: Проверка на админа

            DateTime threshold = DateTime.Now.AddYears(-age);
            return _users.Values.Where(user => user.Birthday.HasValue && user.Birthday.Value < threshold && user.IsActive);
        }

        public void HardDelete(string login)
        {
            // TODO: Проверка на админа

            if (!_users.Remove(login))
                throw new InvalidDataException("Пользователя с таким логином не существует");
        }

        public void SoftDelete(string login, string revokedBy)
        {
            // TODO: Проверка на админа

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            user.Revoke(revokedBy);
        }

        public void RestoreUser(string login)
        {
            // TODO: Проверка на админа

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            user.Restore();
        }

        private bool IsLatinLettersAndDigitsOnly(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            Regex regex = new Regex(@"^[a-zA-Z0-9]+$");
            return regex.IsMatch(input);
        }

        private bool IsLatinAndCyrillicLettersOnly(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            Regex regex = new Regex(@"^[a-zA-Zа-яА-Я]+$");
            return regex.IsMatch(input);
        }
    }
}
