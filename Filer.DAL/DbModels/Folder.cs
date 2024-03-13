using System.ComponentModel.DataAnnotations;

namespace Filer.DAL.DbModels
{
    public class Folder
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string HashName { get; set; }
        public long InFolderId {  get; set; }

        [Required] 
        public long UserId { get; set; }
        public virtual User User { get; set; }

        public ICollection<File>? Files { get; set; }

    }
}
