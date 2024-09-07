using System.ComponentModel.DataAnnotations;

namespace identity.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(40)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MaxLength(40)]
 public  string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(40)]
        [Compare(nameof(Password))]
        public string ConfirmPassWord { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }

    }
}
