﻿using Filer.DAL.DbModels;
using Filer.DAL.Repositories;
using Filer.DAL.Repository;
using Filer.Models;
using Filer.Service;
using Filer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Drawing;

namespace Filer.Controllers
{
    public class FilerController : Controller
    {
        private readonly FileRepository _fileRepository;
        private readonly UserRepository _userRepository;
        private readonly FilerService _service;
        private readonly IWebHostEnvironment _appEnvironment;
        public FilerController(UserRepository userRepository, FilerService service, IWebHostEnvironment hostEnvironment, FileRepository fileRepository, FolderRepository folderRepository)
        {
            _fileRepository = fileRepository;
            _userRepository = userRepository;
            _service = service;
            _appEnvironment = hostEnvironment;
        }
        [RequestSizeLimit(515 * 1024 * 1024)]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile f, long? folder)
        {
            ServiceResponse serviceResponse = await _service.AddFile(HttpContext, f, folder);
            if (!serviceResponse.IsCompleted)
            {
                ModelState.AddModelError(serviceResponse.ErrorEntity, serviceResponse.ErrorMessage);
                return RedirectToAction("Main");
            }

            return RedirectToAction("Main");

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateFolder(string title, long folderId)
        {
            ServiceResponse serviceResponse = await _service.CreateFolder(title, folderId, HttpContext);
            if (!serviceResponse.IsCompleted)
            {
                ModelState.AddModelError(serviceResponse.ErrorEntity, serviceResponse.ErrorMessage);
                return RedirectToAction("Main");
            }
            return RedirectToAction("Main");

        }
        public IActionResult Main(string? src, long? folder)
            
        {

            if (HttpContext.User.Identity.IsAuthenticated)
            {               
                MainDTO dto = _service.CreateMainDTO(src, folder, HttpContext);
                return View(dto);
            }
            return View();
        }
        //Требуется рефакторинг

        [Authorize]
        public FileContentResult OpenFile(long id)
        {
            var file = _fileRepository.Files.FirstOrDefault(f => f.Id == id);
            string path = Path.Combine(_appEnvironment.ContentRootPath, $"Files/{file.HashName[0] + file.HashName[1]}/{file.HashName[2] + file.HashName[3]}/{file.HashName}");
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                byte[] stream = new byte[fileStream.Length];
                fileStream.ReadAsync(stream);
                return new FileContentResult(stream, $"application/{file.Format}");
            }
        }
        [Authorize]
        public IActionResult Profile()
        {
            User? u = _service.GetUserByCookie(HttpContext);
            return View(u);
        }
        [Authorize]
        public IActionResult GetFile(long id)
        {
            var file = _fileRepository.Files.FirstOrDefault(f => f.Id == id);
            string path = Path.Combine(_appEnvironment.ContentRootPath, $"Files/{file.HashName[0] + file.HashName[1]}/{file.HashName[2] + file.HashName[3]}/{file.HashName}");
            return PhysicalFile(path, $"application/{file.Format}", file.Name);
        
        }
    }
}
