using Filer.BL.Services;
using Filer.DAL.DbModels;
using Filer.DAL.Repositories;
using Filer.Services;
using Filer.Services.DTOs;
using Filer.Services.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Filer.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly FileRepository _fileRepository;
        private readonly FilerService _service;
        public AuthController(UserRepository uRepository, FilerService service, FileRepository fRepository)
        {
            _fileRepository = fRepository;
            _userRepository = uRepository;
            _service = service;
        }
        [HttpGet]
        public IActionResult Registration()
        {
            return View(new User());
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string passwordOld, string passwordNew, string passwordConfirm)
        {
            User u = _service.GetUserByCookie(HttpContext);
            if (u.Password == _service.CreateMD5(passwordOld))
            {
                if(passwordNew != passwordConfirm)
                {
                    ModelState.AddModelError("passwordConfirm", "Пароли должны совпадать");
                    return View();
                }
                u.Password = _service.CreateMD5(passwordNew);
                await _userRepository.SaveChangesAsync();
                return RedirectToAction("Profile", "Filer");
            }
            ModelState.AddModelError("passwordOld", "Пароль неверный");
            return View();
        }
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(User user)
        {
            ServiceResponse response = await _service.Registration(user);
            if (!response.IsCompleted)
            {
                ModelState.AddModelError(response.ErrorEntity, response.ErrorMessage);
                return View(user);
            }
            response = await _service.UserSignIn(HttpContext, user);
            if (!response.IsCompleted)
            {
                ModelState.AddModelError(response.ErrorEntity, response.ErrorMessage);
                return View(user);
            }
            return RedirectToAction("Main", "filer");
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View(new UserDto());
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Main", "filer");
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserDto userdto)
        {
            ServiceResponse response = await _service.Login(userdto, HttpContext);
            if (!response.IsCompleted)
            {
                ModelState.AddModelError(response.ErrorEntity, response.ErrorMessage);
                return View(userdto);
            }
            return RedirectToAction("Main", "filer");

        }
    }
}
