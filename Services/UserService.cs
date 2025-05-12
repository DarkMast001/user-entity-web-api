using Microsoft.AspNetCore.Http.HttpResults;
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

        /// <summary>
        /// Создание пользователя по логину, паролю, имени, полу и дате рождения. Доступно только админам.
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользовтаеля</param>
        /// <param name="name">Имя пользователя</param>
        /// <param name="gender">Пол пользователя</param>
        /// <param name="birthday">Дата рождения пользователя</param>
        /// <param name="isAdmin">Будет ли пользователь администратором</param>
        /// <param name="createdBy">Логин пользователя, который создал нового пользователь</param>
        /// <returns>Новый пользователь <see cref="User"/></returns>
        /// <exception cref="InvalidDataException">В случае ввода некорректных данных</exception>
        /// <exception cref="KeyNotFoundException">В случае отсутствия пользователя</exception>
        public User CreateUser(string login, string password, string name, int gender, DateTime? birthday, bool isAdmin, string createdBy)
        {
            if (!IsLatinLettersAndDigitsOnly(login))
                throw new InvalidDataException("Недопустимый логин");

            if (!IsLatinLettersAndDigitsOnly(password))
                throw new InvalidDataException("Недопустимый пароль");

            if (!IsLatinAndCyrillicLettersOnly(name))
                throw new InvalidDataException("Недопустимое имя");

            if (_users.ContainsKey(login))
                throw new InvalidDataException("Логин уже занят");

            if (gender < 0 || gender > 2)
                throw new InvalidDataException("Недопустимое значение поля \"Пол\"");

            if (!_users.ContainsKey(createdBy))
                throw new KeyNotFoundException($"Пользователя с логином {createdBy} не существует");

            User user = new User(login, password, name, gender, birthday, isAdmin, createdBy);
            _users[login] = user;
            return user;
        }

        /// <summary>
        /// Изменение пароля пользователя. Может менять только админ, либо сам пользователь
        /// </summary>
        /// <param name="login">Логин пользователя, чей пароль надо поменять</param>
        /// <param name="newPassword">Новый пароль пользователя</param>
        /// <param name="modifitedBy">Кем текущий пользователь будет изменён</param>
        /// <exception cref="InvalidDataException">В случае ввода недопустимого пароля</exception>
        /// <exception cref="KeyNotFoundException">В случае отсутствия пользователя</exception>
        public void UpdatePassword(string login, string newPassword, string modifitedBy)
        {
            if (!IsLatinLettersAndDigitsOnly(newPassword))
                throw new InvalidDataException("Недопустимый пароль");

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            if (!_users.ContainsKey(modifitedBy))
                throw new KeyNotFoundException($"Пользователя с логином {modifitedBy} не существует");

            _users.Remove(login);
            user.UpdatePassword(newPassword, modifitedBy);
            _users.Add(login, user);
        }

        /// <summary>
        /// Изменение логина пользователя. Может менять только админ, либо сам пользователь
        /// </summary>
        /// <param name="login">Логин пользователя, чей логин надо поменять</param>
        /// <param name="newLogin">Новый логин пользователя</param>
        /// <param name="modifitedBy">Кем текущий пользователь будет изменён</param>
        /// <exception cref="InvalidDataException">В случае ввода недопустимого пароля</exception>
        /// <exception cref="KeyNotFoundException">В случае отсутствия пользователя</exception>
        public void UpdateLogin(string login, string newLogin, string modifitedBy)
        {
            if (!IsLatinLettersAndDigitsOnly(newLogin))
                throw new InvalidDataException("Недопустимый логин");

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            if (!_users.ContainsKey(modifitedBy))
                throw new KeyNotFoundException($"Пользователя с логином {modifitedBy} не существует");

            _users.Remove(login);
            user.UpdateLogin(newLogin, modifitedBy);
            _users.Add(newLogin, user);
        }

        /// <summary>
        /// Изменение имени пользователя. Может менять только админ, либо сам пользователь
        /// </summary>
        /// <param name="login">Логин пользователя, чьё имя надо поменять</param>
        /// <param name="newName">Новое имя пользователя</param>
        /// <param name="modifitedBy">Кем текущий пользователь будет изменён</param>
        /// <exception cref="InvalidDataException">В случае ввода недопустимого имени</exception>
        /// <exception cref="KeyNotFoundException">В случае отсутствия пользователя</exception>
        public void UpdateName(string login, string newName, string modifitedBy)
        {
            if (!IsLatinAndCyrillicLettersOnly(newName))
                throw new InvalidDataException("Недопустимое имя");

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            if (!_users.ContainsKey(modifitedBy))
                throw new KeyNotFoundException($"Пользователя с логином {modifitedBy} не существует");

            _users.Remove(login);
            user.UpdateName(newName, modifitedBy);
            _users.Add(login, user);
        }

        /// <summary>
        /// Изменение пола пользователя. Может менять только админ, либо сам пользователь
        /// </summary>
        /// <param name="login">Логин пользователя, чей пол надо поменять</param>
        /// <param name="newGender">Новый пол пользователя</param>
        /// <param name="modifitedBy">Кем текущий пользователь будет изменён</param>
        /// <exception cref="InvalidDataException">В случае ввода недопустимого имени</exception>
        /// <exception cref="KeyNotFoundException">В случае отсутствия пользователя</exception>
        public void UpdateGender(string login, int newGender, string modifitedBy)
        {
            if (newGender < 0 || newGender > 2)
                throw new InvalidDataException("Недопустимое значение поля \"Пол\"");

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            if (!_users.ContainsKey(modifitedBy))
                throw new KeyNotFoundException($"Пользователя с логином {modifitedBy} не существует");

            _users.Remove(login);
            user.UpdateGender(newGender, modifitedBy);
            _users.Add(login, user);
        }

        /// <summary>
        /// Изменение даты рождения пользователя. Может менять только админ, либо сам пользователь
        /// </summary>
        /// <param name="login">Логин пользователя, чью дату рождения надо поменять</param>
        /// <param name="newBirthday">Новая дата рождения пользователя</param>
        /// <param name="modifitedBy">Кем текущий пользователь будет изменён</param>
        /// <exception cref="KeyNotFoundException">В случае отсутствия пользователя</exception>
        public void UpdateBirthday(string login, DateTime newBirthday, string modifitedBy)
        {
            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            if (!_users.ContainsKey(modifitedBy))
                throw new KeyNotFoundException($"Пользователя с логином {modifitedBy} не существует");

            _users.Remove(login);
            user.UpdateBirthday(newBirthday, modifitedBy);
            _users.Add(login, user);
        }

        /// <summary>
        /// Получить всех активных пользователей. Доступно только админам
        /// </summary>
        /// <returns>Список <see cref="User"/>, отсортированный по дате создания.</returns>
        public IEnumerable<User> GetAllActiveUsers()
        {
            return _users.Values.Where(user => user.IsActive).OrderBy(user => user.CreatedOn);
        }

        /// <summary>
        /// Запрос пользователя по логину. Доступно только админам
        /// </summary>
        /// <param name="login">Логин пользователя, которого надо получить</param>
        /// <returns><see cref="User"/>. Если пользователь не найден - вернёт null.</returns>
        public User? GetByLogin(string login)
        {
            if (_users.TryGetValue(login, out var user))
            {
                return user;
            }

            return null;
        }

        /// <summary>
        /// Запрос пользователя по логину и паролю. Доступно только самому пользователю
        /// </summary>
        /// <param name="login">Логин пользователя, которого надо получить</param>
        /// <param name="password">Пароль пользователя, которого надо получить</param>
        /// <returns><see cref="User"/>. Если пользователь не найден - вернёт null.</returns>
        public User? GetMyUser(string login, string password)
        {
            if (_users.TryGetValue(login, out var user))
            {
                if (user.Password == password)
                    return user;
            }

            return null;
        }

        /// <summary>
        /// Запрос всех пользователей старше определённого возраста. Доступно только админам
        /// </summary>
        /// <param name="age">Возраст, старше которого будет сформирован список</param>
        /// <returns>Список <see cref="User"/> старше определённого возраста.</returns>
        public IEnumerable<User> GetAllUsersOlderThan(int age)
        {
            DateTime threshold = DateTime.Now.AddYears(-age);
            return _users.Values.Where(user => user.Birthday.HasValue && user.Birthday.Value < threshold && user.IsActive);
        }

        /// <summary>
        /// Полное удаление пользователя по логину. Доступно только админам
        /// </summary>
        /// <param name="login">Логин пользователя, которого надо удалить</param>
        /// <exception cref="KeyNotFoundException">В случае отсутствия пользователя</exception>
        /// /// <exception cref="InvalidDataException">При попытке admin удалить самого себя</exception>
        public void HardDelete(string login)
        {
            if (login == "admin")
                throw new InvalidDataException("admin сам себя удалить не может");

            if (!_users.Remove(login))
                throw new KeyNotFoundException("Пользователя с таким логином не существует");
        }

        /// <summary>
        /// Мягкое удаление пользователя. Проставляются поля RevokedOn и RevokedBy. Доступно только админам
        /// </summary>
        /// <param name="login">Логин пользователя, которого надо удалить</param>
        /// <param name="revokedBy">Кем текущий пользователь будет удалён</param>
        /// <exception cref="InvalidDataException">При попытке admin удалить самого себя</exception>
        /// <exception cref="KeyNotFoundException">В случае отсутствия пользователя</exception>
        public void SoftDelete(string login, string revokedBy)
        {
            if (login == "admin")
                throw new InvalidDataException("admin сам себя удалить не может");

            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            user.Revoke(revokedBy);
        }

        /// <summary>
        /// Восстановление пользователя. Очистка полей RevokedOn и RevokedBy. Доступно админам
        /// </summary>
        /// <param name="login">Логин пользователя, которого надо восстановить</param>
        /// <exception cref="KeyNotFoundException">В случае отсутствия пользователя</exception>
        public void RestoreUser(string login)
        {
            if (!_users.TryGetValue(login, out var user))
                throw new KeyNotFoundException($"Пользователя с логином \"{login}\" нет");

            user.Restore();
        }

        /// <summary>
        /// Может ли пользователь войти в систему под логином и паролем
        /// </summary>
        /// <param name="login">Логин пользовтаеля</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="user">Сам пользователь</param>
        /// <returns><see cref=true"/> - пользователь может войти в систему. <see cref=false"/> - пользователь не может войти в систему</returns>
        public bool TryLogin(string login, string password, out User? user)
        {
            if (_users.TryGetValue(login, out user) && user.Password == password && user.IsActive)
            {
                return true;
            }

            user = null;
            return false;
        }

        /// <summary>
        /// Может ли пользователь изменять другого пользователя?
        /// </summary>
        /// <param name="currentLogin">Логин текущего пользователя, который пытается изменить</param>
        /// <param name="targetLogin">Логин изменяемого пользователя</param>
        /// <returns><see cref=true"/> - пользователь может изменить другого пользователя. <see cref=false"/> - не может</returns>
        public bool CanUserModifyUser(string currentLogin, string targetLogin)
        {
            if (!_users.TryGetValue(currentLogin, out User? currentUser))
                return false;

            if (!_users.TryGetValue(targetLogin, out User? targetUser))
                return false;

            if (currentUser.IsAdmin)
                return true;

            return currentLogin == targetLogin && targetUser.IsActive;
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
