using VisitorTrack.Entities;

namespace VisitorTrack.Database
{
    public class UserManager : BaseManager
    {

        public UserManager(string databaseName, string enpointUri, string accountKey)
            : base(databaseName, enpointUri, accountKey)
        {

        }

        protected override string CollectionName => "UserCollection";


    }
}
