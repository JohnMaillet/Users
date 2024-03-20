using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace ServerlessAPI.Models.Interface
{
    public interface IUser
    {
        [Required]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string? Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

    }
}
