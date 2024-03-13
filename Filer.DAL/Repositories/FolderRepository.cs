using Filer.DAL.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Filer.DAL.Repositories
{
    public class FolderRepository
    {
        private FilerDbContext _dbcontext;
        public FolderRepository (FilerDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public IEnumerable<Folder> Folders => _dbcontext.Folders;

        public async Task AddFolder(Folder folder)
        {
            await _dbcontext.Folders.AddAsync(folder);

        }
        public async Task DeleteFolder(long folderid)
        {
            Folder f = await _dbcontext.Folders.Include(f => f.Files).FirstOrDefaultAsync(f => f.Id == folderid);
            if (f != null)
            {
                if(f.Files.Any())
                {
                    foreach (var file in f.Files)
                    {
                        _dbcontext.Files.Remove(file);
                    }
                }    
                List<Folder> CascadeFolders = _dbcontext.Folders.Include(f => f.Files).Where(fld => fld.InFolderId == f.Id).ToList();
                if (CascadeFolders.Any())
                {
                    foreach (Folder folder in CascadeFolders) 
                    {
                        await DeleteFolder(folder.Id);
                    }
                }
                _dbcontext.Folders.Remove(f);
            }
        }
        public async Task SaveChangesAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }
        public void SaveChanges()
        {
            _dbcontext.SaveChanges();
        }
        public long FolderContains(Folder folder)
        {
            Folder? f = _dbcontext.Folders.FirstOrDefault(f => f.HashName == folder.HashName);
            if (f != null)
                return f.Id;

            return -1;
        }
    }
}
