using System.ComponentModel.DataAnnotations;

namespace Filer.DAL.DbModels
{
    public class User
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно!")]
        [MaxLength(32)]
        [MinLength(4)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно!")]
        [EmailAddress]
        [MaxLength(255)]

        public string Email { get; set; }


        [Required(ErrorMessage = "Поле обязательно!")]
        [MinLength(6)]
        [MaxLength(255)]
        public string Password { get; set; }



        public ICollection<File> Files { get; set; }
        public ICollection<Folder> Folders { get; set; }

    }
}
