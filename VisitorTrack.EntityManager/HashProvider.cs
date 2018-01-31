using System;
using System.Security.Cryptography;
using System.Text;

namespace VisitorTrack.EntityManager
{
    public class HashProvider
    {
        private const string PostSalt = "_buffer_9#00!#8423-12834)*@$920*";
        private const string PreSalt = "visitor_track_salt_";

        public static string Hash(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            var data = Encoding.UTF8.GetBytes($"{PreSalt}{value}{PostSalt}");

            data = new SHA256CryptoServiceProvider().ComputeHash(data);

            return Convert.ToBase64String(data);
        }
    }
}