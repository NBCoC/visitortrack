namespace VisitorTrack.EntityManager.Dtos
{
    public class TokenDto
    {
        public string Email { get; }

        public string Token { get; }

        public TokenDto(string email, string token)
        {
            Email = email;
            Token = token;
        }
    }
}