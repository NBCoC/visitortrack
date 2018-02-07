using VisitorTrack.Entities.Models;

namespace VisitorTrack.Entities.Dtos
{
    public class UpsertUserDto
    {
        public string EmailAddress { get; set; }

        public UserRoleEnum RoleId { get; set; }
        
        public string DisplayName { get; set; }
    }
}