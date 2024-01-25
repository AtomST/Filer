using Filer.DAL;
using Filer.DAL.DbModels;
using Filer.DAL.Repositories;
using Filer.DAL.Repository;
using Filer.Models;
using Filer.Service;
using Filer.Services;
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
            return RedirectToAction("main", "filer");
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View(new UserDto());
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("main", "filer");
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
            return RedirectToAction("main", "filer");

        }
    }
}
