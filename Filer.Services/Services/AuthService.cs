using Filer.DAL.DbModels;
using Filer.DAL.Repositories;
using Filer.Services.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Filer.BL.Services
{
    internal class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly FileRepository _fileRepository;
        private readonly FolderRepository _folderRepository;
        private readonly ILogger<AuthService> _logger;
        public AuthService(UserRepository uRepository, ILogger<AuthService> logger, FileRepository fRepository, FolderRepository folderRepository)
        {
            _fileRepository = fRepository;
            _userRepository = uRepository;
            _folderRepository = folderRepository; ;
            _logger = logger;
        }
    }
}
