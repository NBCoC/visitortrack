namespace VisitorTrack.Entities.Dtos
{
    public class UserAuthenticatedDto : UserDto
    {
        public string Token { get; set; }
    }
}