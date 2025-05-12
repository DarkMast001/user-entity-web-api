using Microsoft.AspNetCore.Mvc;
using UserEntityWebAPI.DTO;
using UserEntityWebAPI.Services;
using UserEntityWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UserEntityWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("active")]
        [Authorize(Roles = "admin")]
        public IActionResult GetActiveUsers()
        {
            var users = _userService.GetAllActiveUsers();
            return Ok(users.Select(u => new
            {
                u.Id,
                u.Login,
                u.Name,
                u.Gender,
                u.Birthday,
                u.IsAdmin,
                u.CreatedOn,
                u.CreatedBy,
                u.ModifiedOn,
                u.ModifiedBy,
            }));
        }

        [HttpGet("bylogin")]
        [Authorize(Roles = "admin")]
        public IActionResult GetUserByLogin([FromQuery] GetByLoginRequest request)
        {
            User? user = _userService.GetByLogin(request.Login);
            if (user == null)
                return NotFound("Такой пользователь не найден");
            else
                return Ok(new {
                    user.Login,
                    user.Name, 
                    user.Gender, 
                    user.Birthday, 
                    user.IsAdmin,
                    user.CreatedOn,
                    user.CreatedBy,
                    user.ModifiedOn,
                    user.ModifiedBy,
                    user.RevokedOn,
                    user.RevokedBy,
                    user.IsActive
                });
        }

        [HttpGet("myuser")]
        public IActionResult GetMyUser([FromQuery] string login, [FromQuery] string password)
        {
            string currentLogin = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "none";

            if (currentLogin != login)
                return Forbid("Вы можете получить только свои данные");

            User? user = _userService.GetMyUser(login, password);

            if (user == null)
                return NotFound("Такого пользователя нет или пароль неверен");

            if (!user.IsActive)
                return BadRequest("Ваш пользователь не активен");

            return Ok(new
            {
                user.Login,
                user.Name,
                user.Gender,
                user.Birthday,
                user.CreatedOn,
                user.IsActive
            });
        }

        [HttpGet("olderthan")]
        [Authorize(Roles = "admin")]
        public IActionResult GetAllUsersOlderThan([FromQuery] int age)
        {
            var users = _userService.GetAllUsersOlderThan(age);

            if (users.Count() == 0)
                return NotFound("Подходящих пользователей нет");

            return Ok(users.Select(user => new
            {
                user.Login,
                user.Name,
                user.Gender,
                user.Birthday,
                user.CreatedOn,
                user.IsActive
            }));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                string currentLogin = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "none";

                User user = _userService.CreateUser(
                    login: request.Login,
                    password: request.Password,
                    name: request.Name,
                    gender: request.Gender,
                    birthday: request.Birthday,
                    isAdmin: request.IsAdmin,
                    createdBy: currentLogin
                );

                return Ok(new { user.Id, user.Login });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{login}/name")]
        public IActionResult UpdateName(string login, [FromBody] UpdateNameRequest request)
        {
            string currentLogin = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "none";

            if (!_userService.CanUserModifyUser(currentLogin, login))
            {
                return Forbid("У вас нет прав изменять этого пользователя, либо пользовтаель не активен.");
            }

            try
            {
                _userService.UpdateName(login, request.Name, currentLogin);

                return Ok("Имя успешно изменено");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{login}/password")]
        public IActionResult UpdatePassword(string login, [FromBody] UpdatePasswordRequest request)
        {
            string currentLogin = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "none";

            if (!_userService.CanUserModifyUser(currentLogin, login))
            {
                return Forbid("У вас нет прав изменять этого пользователя, либо пользовтаель не активен.");
            }

            try
            {
                _userService.UpdatePassword(login, request.NewPassword, currentLogin);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{login}/login")]
        public IActionResult UpdateLogin(string login, [FromBody] UpdateLoginRequest request)
        {
            string currentLogin = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "none";

            if (!_userService.CanUserModifyUser(currentLogin, login))
            {
                return Forbid("У вас нет прав изменять этого пользователя, либо пользовтаель не активен.");
            }

            try
            {
                _userService.UpdateLogin(login, request.NewLogin, currentLogin);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{login}/gender")]
        public IActionResult UpdateGender(string login, [FromBody] UpdateGenderRequest request)
        {
            string currentLogin = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "none";

            if (!_userService.CanUserModifyUser(currentLogin, login))
            {
                return Forbid("У вас нет прав изменять этого пользователя, либо пользовтаель не активен.");
            }

            try
            {
                _userService.UpdateGender(login, request.NewGender, currentLogin);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{login}/birthday")]
        public IActionResult UpdateBirthday(string login, [FromBody] UpdateBirthdayRequest request)
        {
            string currentLogin = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "none";

            if (!_userService.CanUserModifyUser(currentLogin, login))
            {
                return Forbid("У вас нет прав изменять этого пользователя, либо пользовтаель не активен.");
            }

            try {
                _userService.UpdateBirthday(login, request.NewBirthday, currentLogin);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("soft/{login}")]
        [Authorize(Roles = "admin")]
        public IActionResult SoftDelete(string login)
        {
            string currentLogin = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "none";

            try
            {
                _userService.SoftDelete(login, currentLogin);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("hard/{login}")]
        [Authorize(Roles = "admin")]
        public IActionResult HardDelete(string login)
        {
            try
            {
                _userService.HardDelete(login);

                return Ok("Пользователь успешно удалён");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{login}/restore")]
        [Authorize(Roles = "admin")]
        public IActionResult RestoreUser(string login)
        {
            try
            {
                _userService.RestoreUser(login);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
