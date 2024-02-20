using Filer.DAL.DbModels;
using File = Filer.DAL.DbModels.File;

namespace Filer.Models
{
    public class MainDTO
    {
        public IEnumerable<DAL.DbModels.File> files { get; set; }
        public IEnumerable<Folder> folders { get; set; }
        public string? SrcParam { get; set; }
        public long? folderId { get; set; } 
    }
}
