using JWTAuthTest.Entities;
using JWTAuthTest.Models;
using JWTAuthTest.Services;
using JWTAuthTest.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JWTAuthTest.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            Debug.WriteLine(model);
            var response = _userService.Authenticate(model);
            if (response == null) return BadRequest(new { message = "Username o Password incorrectos, reviselos antes de continuar" });
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> PostUser(User user)
        {
            if (user == null) return BadRequest(new { message = "Debes introducir los datos requeridos para continuar" });
            var token = await _userService.CreateUser(user);
            if (token == null) return BadRequest(new { message = "Algo salió mal, intentalo de nuevo" });
            await _userService.SaveChanges();
            return Ok(new {message = "Usuario introducido", token.Token});
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_userService.GetAll());
        }
     }
}
