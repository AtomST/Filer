
using Filer.BL.Services;
using Filer.DAL.DbModels;
using Filer.DAL.Repositories;
using Filer.Services;
using Filer.Services.DTOs;
using Filer.Services.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Filer.Controllers
{
    public class FilerController : Controller
    {
        private readonly FileRepository _fileRepository;
        private readonly UserRepository _userRepository;
        private readonly FolderRepository _folderRepository;
        private readonly FilerService _service;
        private readonly IWebHostEnvironment _appEnvironment;
        public FilerController(UserRepository userRepository, FilerService service, IWebHostEnvironment hostEnvironment, FileRepository fileRepository, FolderRepository folderRepository)
        {
            _fileRepository = fileRepository;
            _userRepository = userRepository;
            _service = service;
            _appEnvironment = hostEnvironment;
            _folderRepository = folderRepository;
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
                ViewBag.ErrorEntity = serviceResponse.ErrorMessage;
                return RedirectToAction("Main");
            }

            return RedirectToAction("Main");

        }
        [RequestSizeLimit(515 * 1024 * 1024)]
        public async Task<IActionResult> AddFolder(IFormFileCollection f, long folder)
        {
            ServiceResponse serviceResponse = await _service.AddFolder(HttpContext, f, folder);
            if (!serviceResponse.IsCompleted)
            {
                ModelState.AddModelError(serviceResponse.ErrorEntity, serviceResponse.ErrorMessage);
                return RedirectToAction("Main");
            }

            return RedirectToAction("Main");

        }
        [Authorize]
        public async Task<IActionResult> DeleteFolder(long folderid)
        {
            await _folderRepository.DeleteFolder(folderid);
            _folderRepository.SaveChanges();
            return RedirectToAction("Main");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateFolder(string title, long folderId)
        {
            AddFolderResponse serviceResponse = await _service.CreateFolder(title, folderId, HttpContext, _service.GetUserByCookie(HttpContext).Id);
            if (!serviceResponse.IsCompleted)
            {
                ModelState.AddModelError(serviceResponse.ErrorEntity, serviceResponse.ErrorMessage);
                return RedirectToAction("Main");
            }
            return RedirectToAction("Main");

        }
        public IActionResult Main(string? src, long folder)
            
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
            string path = Path.Combine(_appEnvironment.ContentRootPath, file.GetPath());
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
            string path = Path.Combine(_appEnvironment.ContentRootPath, file.GetPath());
            return PhysicalFile(path, $"application/{file.Format}", file.Name);
        
        }
        [Authorize]
        public async Task<IActionResult> DeleteFile(long id)
        {
            var file = _fileRepository.Files.FirstOrDefault(f => f.Id == id);
            if(file != null)
            {
                _fileRepository.DeleteFile(file);
                FileInfo f = new FileInfo(file.GetPath());
                if(f.Exists)
                {
                    f.Delete();
                }
                await _fileRepository.SaveChangesAsync();
            }
            return RedirectToAction("Main");

        }
        [Authorize]
        public IActionResult GetFolder(long id)
        {
            var file = _fileRepository.Files.FirstOrDefault(f => f.Id == id);
            string path = Path.Combine(_appEnvironment.ContentRootPath, file.GetPath());
            return PhysicalFile(path, $"application/{file.Format}", file.Name);

        }
    }
}
