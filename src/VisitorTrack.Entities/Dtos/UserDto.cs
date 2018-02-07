using VisitorTrack.Entities.Models;

namespace VisitorTrack.Entities.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }  
        
        public string EmailAddress { get; set; }

        public string RoleName => RoleId.ToString();

        public UserRoleEnum RoleId { get; set; }

        public string DisplayName { get; set; }

        public bool IsAdmin => RoleId == UserRoleEnum.Admin;

        public bool IsEditor => IsAdmin || RoleId == UserRoleEnum.Editor;
    }
}