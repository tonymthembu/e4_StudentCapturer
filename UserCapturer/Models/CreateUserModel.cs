using System.ComponentModel.DataAnnotations;

namespace UserCapturer.Models
{
    public class CreateUserModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        public required string Surname { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Cellphone is required.")]
        public required string Cellphone { get; set; }
    }
}
