using ServerlessAPI.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace ServerlessAPI.Entities.Interface
{

    public interface IUserEntity
    {
        public string Id { get; set; }
        public string? AccountCreationTimeStamp { get; set; }
        public string? LastLoginTime { get; set; }
        public string? LastModifiedBy { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
    }
}