using ServerlessAPI.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace ServerlessAPI.Models.Implementations
{
    public class User: IUser
    {
        [Required]
        [MaxLength(15, ErrorMessage = "User Name cannot exceed 15 characters")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        [MaxLength(300, ErrorMessage = "Email address cannot exceed 300 characters")]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "First Name cannot exceed 300 characters")]
        public string FirstName { get; set; }
        [MaxLength(300, ErrorMessage = "Middle Name cannot exceed 300 characters")]
        public string? MiddleName { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "Last Name cannot exceed 300 characters")]
        public string LastName { get; set; }

    }
}
