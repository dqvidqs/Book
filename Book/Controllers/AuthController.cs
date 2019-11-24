using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BookAPI.Auth;
using BookAPI.Data;
using BookAPI.Models;
using Microsoft.AspNetCore.Http;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly BookContext _context;
        private readonly IConfiguration configuration;
        private TokenProvider tokenProvider;

        public AuthController(BookContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
            tokenProvider = new TokenProvider(configuration);
        }
        [Route("register")]
        [HttpPost]
        public IActionResult Store([FromBody] Register model)
        {
            var User = _context.Users.Where(a => a.email == model.email).FirstOrDefault();
            if (User != null)
            {
                return Json(new
                {
                    value = "User exist!"
                });
            }

            User user = new User()
            {
                email = model.email,
                password = tokenProvider.Hash(model.password),
                role = model.role,
                name = model.name

            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return Json(new
            {
                value = user,
            });
        }
        [Route("login")]
        [HttpPost]
        public IActionResult LoginUser([FromBody] Login model)
        {
            var User = _context.Users.Where(a => a.email == model.email).FirstOrDefault();
            if (User == null)
            {
                return Json(new
                {
                    value = "User does not exist",
                });
            }
            if (tokenProvider.Verify(User.password,model.password))
            {
                return Json(new
                {
                    value = "password not match!",
                });
            }
            var userToken = tokenProvider.LoginUser(User);
            if (userToken != null)
            {
                HttpContext.Session.SetString("JWToken", userToken);
            }
            return Json(new
            {
                value = userToken,
            });
        }
        [Route("home")]
        [HttpGet]
        public IActionResult Home()
        {
            return Json(new
            {
                value = "Home",
            });
        }
        [Route("permission")]
        [HttpGet]
        public IActionResult NoPer()
        {
            return Json(new
            {
                value = "Opp! NO Permissions",
            });
        }
    }
}