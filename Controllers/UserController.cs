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
                u.CreatedBy
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
                return Forbid("У вас нет прав изменять этого пользователя");
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

        //[HttpGet("debug")]
        //public IActionResult DebugUsers()
        //{
        //    return Ok(_userService.GetAllUsers());
        //}

        // Переделать следующие методы под авторизацию

        [HttpPut("{login}/password")]
        public IActionResult UpdatePassword(string login, [FromBody] UpdatePasswordRequest request)
        {
            // TODO: это должен делать и admin, и лично сам пользователь (если пользователь активен (отсутствует RevokedOn))
            try
            {
                // TODO: заменить admin
                _userService.UpdatePassword(login, request.NewPassword, "admin");

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
            // TODO: это должен делать и admin, и лично сам пользователь (если пользователь активен (отсутствует RevokedOn))
            try
            {
                // TODO: заменить admin
                _userService.UpdateLogin(login, request.NewLogin, "admin");

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
            // TODO: это должен делать и admin, и лично сам пользователь (если пользователь активен (отсутствует RevokedOn))
            try
            {
                // TODO: заменить admin
                _userService.UpdateGender(login, request.NewGender, "admin");

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
            // TODO: это должен делать и admin, и лично сам пользователь (если пользователь активен (отсутствует RevokedOn))
            try
            {
                // TODO: изменить admin
                _userService.UpdateBirthday(login, request.NewBirthday, "admin");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{login}")]
        public IActionResult DeleteUser(string login)
        {
            // TODO: это должен делать только admin
            try
            {
                // TODO: заменить admin
                _userService.SoftDelete(login, "admin");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{login}/restore")]
        public IActionResult RestoreUser(string login)
        {
            // TODO: это должен делать только admin
            try
            {
                // TODO: проверять что доступно только админам
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
