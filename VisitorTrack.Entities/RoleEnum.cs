using System.ComponentModel;

namespace VisitorTrack.Entities
{
    public enum RoleEnum
    {
        [Description("Admin")] Admin = 1,
        [Description("Editor")] Editor = 2,
        [Description("Viewer")] Viewer = 3
    }
}
