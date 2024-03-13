using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Filer.DAL.DbModels
{
    public class File
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string HashName { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public float Size { get; set; }
        [MaxLength(10)]
        public string Format { get; set; }




        //Relationship
        [Required]
        public long UserId { get; set; }
        public virtual User User { get; set; }



        public long? FolderId { get; set; }
        public Folder Folder { get; set; }

        public string GetPath()
        {
            return $"Files/{HashName[0] + HashName[1]}/{HashName[2] + HashName[3]}/{HashName}";
        }
    }
}
