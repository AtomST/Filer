using Filer.DAL.DbModels;
using Filer.DAL.Repositories;
using Filer.DAL.Repository;
using Filer.Models;
using Filer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Filer.Service
{
    public class FilerService
    {
        private readonly UserRepository _userRepository;
        private readonly FileRepository _fileRepository;
        private readonly FolderRepository _folderRepository;
        private readonly ILogger<FilerService> _logger;
        public FilerService(UserRepository uRepository, ILogger<FilerService> logger, FileRepository fRepository, FolderRepository folderRepository)
        {
            _fileRepository = fRepository;
            _userRepository = uRepository;
            _folderRepository = folderRepository;
            _logger = logger;
        }

        public async Task<ServiceResponse> AddFile(HttpContext context, IFormFile f, long? folder)
        {
            User? u = GetUserByCookie(context);
            DAL.DbModels.File file = new DAL.DbModels.File();
            file.Size = (float)Math.Round((f.Length / 1024.0 / 1024), 2);
            file.CreationDate = DateTime.Now;
            file.Name = f.FileName;
            file.FolderId = folder;
            file.HashName = CreateMD5(u.Name + u.Id + file.Name);
            file.Format = Path.GetExtension(f.FileName).TrimStart('.');
            string path = $"Files/{file.HashName[0] + file.HashName[1]}/{file.HashName[2] + file.HashName[3]}";
            try
            {
                file.User = u;
                await _fileRepository.AddFile(file);
                Directory.CreateDirectory(path);
                path += $"/{file.HashName}";

                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    await f.CopyToAsync(fileStream);
                }
                await _fileRepository.SaveChangesAsync();
                return new ServiceResponse { IsCompleted = true };
            }

            catch (Exception ex) 
            { 
                ServiceResponse response = new ServiceResponse { IsCompleted = false, ErrorEntity = "file", ErrorMessage = "При добавлении файла произошла ошибка" };
                _logger.LogError(response.ErrorMessage + ": " + ex.Message);
                return response;
            }
        }

        public async Task<ServiceResponse> Login(UserDto userdto, HttpContext context)
        {
            User? user = GetUserByEmail(userdto.Email);
            if (user == null)
            {
                return new ServiceResponse { IsCompleted = false, ErrorEntity = "Email", ErrorMessage = "Пользователь с таким Email не найден" };
            }
            if(user.Password != CreateMD5(userdto.Password))
            {
                return new ServiceResponse { IsCompleted = false, ErrorEntity = "Password", ErrorMessage = "Неверный пароль" };
            }
            await UserSignIn(context, user);
            return new ServiceResponse { IsCompleted = true };

        }

        public User? GetUserByEmail(string email)
        {
            return _userRepository.Users.FirstOrDefault(u => u.Email == email);
        }

        public async Task<ServiceResponse> UserSignIn(HttpContext context, User user)
        {
            try
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("Id", user.Id.ToString())
                };
                ClaimsIdentity identity = new ClaimsIdentity(claims, "UData");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await context.SignInAsync("UData",principal);
                return new ServiceResponse { IsCompleted = true };
                
            }
            catch(Exception ex)
            {
                ServiceResponse serviceResponse = new ServiceResponse { IsCompleted = false, ErrorMessage = "При авторизации пользователя произошла ошибка", ErrorEntity = " " };
                _logger.LogError($"{serviceResponse.ErrorMessage} : {ex.Message}");
                return serviceResponse;
            }
        }

        public async Task<ServiceResponse> CreateFolder(string title, long folderId, HttpContext context)
        {
            Folder folder = new Folder();
            folder.Name = title;
            folder.HashName = CreateMD5(title);
            folder.UserId = Convert.ToInt64(context.User.Claims.FirstOrDefault(x => x.Type == "Id").Value);
            if (folderId != null)
            {
                folder.InFolderId = folderId;
            }
            try
            {
                await _folderRepository.AddFolder(folder);
                await _folderRepository.SaveChangesAsync();
                return new ServiceResponse { IsCompleted = true };
            }
            catch (Exception ex)
            {
                ServiceResponse serviceResponse = new ServiceResponse { IsCompleted = false, ErrorMessage = "При добавлении папки произошла ошибка, попробуйте еще раз", ErrorEntity = "title" };
                _logger.LogError($"{serviceResponse.ErrorMessage} : {ex.Message}");
                return serviceResponse;
            }
        }

        public MainDTO CreateMainDTO(string? src,long? folderId, HttpContext context)
        {
            MainDTO mainDTO = new MainDTO();
            mainDTO.folderId = folderId;
            mainDTO.SrcParam = src;
            User? user = _userRepository.UsersWithData.FirstOrDefault(x => x.Id.ToString() == context.User.Claims.FirstOrDefault(x => x.Type == "Id").Value);
            if (user == null)
            {
                return mainDTO;
            }
            mainDTO.files = user.Files;
            mainDTO.folders = user.Folders;
            if(src != null)
            {
                mainDTO.files = mainDTO.files.Where(x => x.Name.Contains(src)).ToList();
                mainDTO.folders = mainDTO.folders.Where(x => x.Name.Contains(src)).ToList();
            }
            if(folderId == null)
            {
                mainDTO.files = mainDTO.files.Where(x => x.FolderId == null).ToList();
                mainDTO.folders = mainDTO.folders.Where(x => x.InFolderId == 0).ToList();
                return mainDTO;
            }
            mainDTO.files = mainDTO.files.Where(x => x.FolderId == folderId).ToList(); 
            mainDTO.folders = mainDTO.folders.Where(x => x.InFolderId == folderId).ToList();
            return mainDTO;
        }

        public User GetUserByCookie(HttpContext context)
        {
            string userid = context.User.Claims.First(x => x.Type == "Id").Value;
            return _userRepository.Users.FirstOrDefault(x => x.Id.ToString() == userid);

        }

        public async Task<ServiceResponse> Registration(User user)
        {
            try
            {
                if (_userRepository.UserContains(user))
                {
                    return new ServiceResponse { ErrorEntity = "Email", ErrorMessage = "Пользователь с таким Email уже существует" };
                }
                user.Password = CreateMD5(user.Password);
                await _userRepository.AddUser(user);
                return new ServiceResponse { IsCompleted = true };

            }
            catch(Exception ex)
            {
                ServiceResponse serviceResponse = new ServiceResponse { IsCompleted = false, ErrorMessage = "При добавлении пользователя произошла ошибка" , ErrorEntity = " "};
                _logger.LogError($"{serviceResponse.ErrorMessage} : {ex.Message}");
                return serviceResponse;
            }
        }

        public string CreateMD5(string data)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes($"{data}.{"h1249120hfmdnsvq4xcjknAfsaoifdasd"}"));
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    stringBuilder.Append(hash[i].ToString("X2"));
                }
                return stringBuilder.ToString();
            }
        }
    }
}
