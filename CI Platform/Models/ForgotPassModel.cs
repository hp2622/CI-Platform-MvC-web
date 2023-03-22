using System.ComponentModel.DataAnnotations;

namespace CI_Platform.Models
{
    public class ForgetPass
    {
        [Required(ErrorMessage = "please enter Email")]
        public string? Email { get; set; }

    }
}




