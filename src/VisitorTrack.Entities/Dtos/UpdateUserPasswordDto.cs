namespace VisitorTrack.Entities.Dtos
{
    public class UpdateUserPasswordDto
    {
        public string NewPassword { get; set; }

        public string OldPassword { get; set; }
    }
}