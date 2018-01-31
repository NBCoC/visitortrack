using System.Linq;

namespace VisitorTrack.EntityManager
{
    public static class Extensions
    {
        public static TEntity SingleEntity<TEntity>(this IQueryable<TEntity> query)
        {   
            TEntity entity = default(TEntity);

            foreach (var item in query)
            {
                entity = item;
                break;
            }

            return entity;
        }
    }
}