namespace CI_Platform.Models
{
    public class ResetpassModel
    {
        public string? Email { get; set; }
        
        public string? Token { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        //public DateTime UpdatedAt { get; set; }
        
    }
}
