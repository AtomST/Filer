﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Filer.DAL.Repositories
{
    public class FileRepository
    {
        private FilerDbContext _dbcontext;
        private ILogger<FileRepository> _logger;
        public FileRepository(FilerDbContext dbContext, ILogger<FileRepository> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
        }
        public IEnumerable<DbModels.File> Files => _dbcontext.Files;

        public async Task AddFile(DbModels.File file)
        {
            await _dbcontext.Files.AddAsync(file);

        }
        public void DeleteFile(DbModels.File file)
        {
            _dbcontext.Files.Remove(file);
        }
        public async Task SaveChangesAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }
    }
}
