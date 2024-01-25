using Filer.DAL.DbModels;

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
        public async Task SaveChangesAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }
    }
}
