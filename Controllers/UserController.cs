using Microsoft.AspNetCore.Mvc;
using UserEntityWebAPI.DTO;
using UserEntityWebAPI.Services;
using UserEntityWebAPI.Models;

namespace UserEntityWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("active")]
        public IActionResult GetActiveUsers()
        {
            return Ok("Successs");
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                User user = _userService.CreateUser(
                    login: request.Login,
                    password: request.Password,
                    name: request.Name,
                    gender: request.Gender,
                    birthday: request.Birthday,
                    isAdmin: request.IsAdmin,
                    "admin" // TODO: заменить на реального пользователя после внедрения аутентификации
                );

                return Ok(new { user.Id, user.Login });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateName(string login, [FromBody] UpdateNameRequest request)
        {
            try
            {
                // TODO: заменить на реального пользователя после внедрения аутентификации
                _userService.UpdateName(login, request.Name, "admin");

                return Ok("Имя успешно изменено");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
