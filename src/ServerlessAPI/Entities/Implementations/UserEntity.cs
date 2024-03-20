using Amazon.DynamoDBv2.DataModel;
using ServerlessAPI.Entities.Interface;
using System.ComponentModel.DataAnnotations;
namespace ServerlessAPI.Entities.Implementations
{

    // <summary>
    /// Map the User Class to DynamoDb Table
    /// To learn more visit https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DeclarativeTagsList.html
    /// </summary>
    public class UserEntity : IUserEntity
    {
        ///<summary>
        /// Map c# types to DynamoDb Columns 
        /// to learn more visit https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/MidLevelAPILimitations.SupportedTypes.html
        /// <summary>
        public string Id { get; set; }
        [DataType(DataType.DateTime)]
        public string? AccountCreationTimeStamp { get; set; }

        [DataType(DataType.DateTime)]
        public string? LastLoginTime { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(15, ErrorMessage = "Last Modified By cannot exceed 15 characters")]
        public string? LastModifiedBy { get; set; }
        [Required]
        [MaxLength(15, ErrorMessage = "User Name cannot exceed 15 characters")]
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        [MaxLength(300, ErrorMessage = "Email address cannot exceed 300 characters")]
        public string? Email { get; set; }
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