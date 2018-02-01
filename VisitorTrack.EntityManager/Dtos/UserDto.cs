using VisitorTrack.EntityManager.Models;

namespace VisitorTrack.EntityManager.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public RoleEnum RoleId { get; set; }

        public string RoleName => RoleId.ToString();

        public string DisplayName { get; set; }

        public bool IsAdmin => RoleId == RoleEnum.Admin;

        public bool IsEditor => IsAdmin || RoleId == RoleEnum.Editor;
    }
}