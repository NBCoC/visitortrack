using Microsoft.Azure.Documents.Client;
using VisitorTrack.Entities;
using System.Linq;
using VisitorTrack.EntityManager.Dtos;
using System;
using System.Threading.Tasks;

namespace VisitorTrack.EntityManager
{
    public class UserManager : BaseManager
    {

        public UserManager(string databaseId, string enpointUrl, string accountKey)
            : base(databaseId, enpointUrl, accountKey)
        { }

        protected override string CollectionId => "UserCollection";

        public async Task<TokenDto> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            var sql = $"SELECT TOP 1 * FROM UserCollection WHERE UserCollection.Email = '{CanonicalizeEmail(email)}' AND UserCollection.Password = '{HashProvider.Hash(password)}'";

            var entity =
                DocumentClient.CreateDocumentQuery<User>(GetCollectionUri(), sql).SingleEntity<User>();

            if (entity == null)
                throw new InvalidOperationException($"User with email {email} not found or password is incorrect.");

            entity.Token = Guid.NewGuid().ToString();

            await UpdateEntityAsync(entity.Id, entity);

            return new TokenDto(entity.Email, entity.Token);
        }

        public UserDto Find(string entityId)
        {
            if (string.IsNullOrEmpty(entityId))
                throw new ArgumentNullException(nameof(entityId));

            var entity =  
                DocumentClient.CreateDocumentQuery<User>(GetCollectionUri())
                    .Where(x => x.Id == entityId)
                    .SingleEntity<User>();

            if (entity == null)
                throw new InvalidOperationException($"User with ID '{entityId}' not found");

            return new UserDto()
                    {
                        Id = entity.Id,
                        RoleId = entity.Role,
                        Email = entity.Email,
                        DisplayName = entity.DisplayName
                    };
        }

        public async Task<string> CreateAsync(UpsertUserDto dto, string password)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrEmpty(dto.DisplayName))
                throw new InvalidOperationException("Display Name is required");

            if (string.IsNullOrEmpty(dto.Email))
                throw new InvalidOperationException("Email is required");

            var entity = new User()
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                Role = dto.RoleId,
                Password = HashProvider.Hash(password)
            };

            return await CreateEntityAsync(entity);
        }
    }
}
