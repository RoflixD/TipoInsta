using System.ComponentModel.DataAnnotations;

namespace Main.Models.User
{
    public class CreateUserModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Compare(nameof(RetryPassword))]
        public string Password { get; set; }
        [Required]
        public string RetryPassword { get; set; }
        public DateTimeOffset BirthDate { get; set; }

        public CreateUserModel(string name, string email, string password, string retryPassword, DateTimeOffset birthDate)
        {
            Name = name;
            Email = email;
            Password = password;
            RetryPassword = retryPassword;
            BirthDate = birthDate;
        }
    }
}
