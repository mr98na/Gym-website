using System.ComponentModel.DataAnnotations;

namespace Gym.Models
{
    public class Signup
    {
        [Required(ErrorMessage = "*")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "*")]
        public string Name { get; set; }
        [Required(ErrorMessage = "*")]
        [RegularExpression("^[0-9]{10}", ErrorMessage = "Required 10 Numbers")]
        public string Mobile { get; set; }
        public string Type { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Must be {0} from {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match password confirmation.")]
        public string ConfirmPassword { get; set; }
    }
}
