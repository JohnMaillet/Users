using Amazon.Runtime.Internal.Util;
using Bogus;
using ServerlessAPI.Entities;
using ServerlessAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessAPI.Tests
{
    internal class MockUserRepository : IUserRepository
    {
        private readonly Faker<UserEntity> fakeEntity;

        public MockUserRepository()
        {
            var now = DateTimeOffset.UtcNow;
            fakeEntity = new Faker<UserEntity>()
                .RuleFor(o=> o.UserName, f => f.Person.UserName )
                .RuleFor(o=> o.Email, f => f.Person.Email)
                .RuleFor(o=> o.FirstName, f=> f.Person.FirstName)
                .RuleFor(o => o.LastName, f => f.Person.LastName)
                .RuleFor(o => o.AccountCreationTimeStamp, f => {
                    if (f.IndexFaker % 3 == 0) now = now.AddMilliseconds(10);
                    return now.ToUnixTimeMilliseconds().ToString();
                } );
        }

        public Task<bool> CreateAsync(UserEntity user)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(UserEntity user)
        {
            return Task.FromResult(true);
        }

        public Task<IList<UserEntity>> GetUsersAsync(int limit = 10)
        {
            IList<UserEntity> users = fakeEntity.Generate(limit).ToList();

            return Task.FromResult(users);
        }

        public Task<UserEntity?> GetByIdAsync(string id)
        {
            _ = fakeEntity.RuleFor(o => o.Id, f => id);
            var user = fakeEntity.Generate() ?? null;

            return Task.FromResult(user);
        }

        public Task<bool> UpdateAsync(UserEntity user)
        {
            return Task.FromResult(true);
        }
    }
}
