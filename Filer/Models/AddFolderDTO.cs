using Filer.DAL.DbModels;

namespace Filer.Models
{
    public class AddFolderDTO
    {
        public List<string> foldersTitle { get; set; }
        public string FileName { get; set; }
    }
}
