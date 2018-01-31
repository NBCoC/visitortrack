using VisitorTrack.Entities;

namespace VisitorTrack.EntityManager.Dtos
{
    public class UpsertUserDto
    {
        public string Email { get; set; }

        public RoleEnum RoleId { get; set; }

        public string DisplayName { get; set; }
    }
}