using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Pengqian.NetworkDisk.Infrastructure;

namespace Pengqian.NetworkDisk.DbAccess
{
    public static class DbExtensions
    {
        /// <summary>
        /// 根据ObjectId快速查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T Find<T>(this IMongoCollection<T> collection, ObjectId id) where T : IBaseEntity
        {
            var filter = Builders<T>.Filter.Eq(new StringFieldDefinition<T, ObjectId>("Id"), id);
            return collection.Find(filter).SingleOrDefault();
        }

        /// <summary>
        /// 异步 根据ObjectId快速查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<T> FindAsync<T>(this IMongoCollection<T> collection, ObjectId id) where T : IBaseEntity
        {
            var filter = Builders<T>.Filter.Eq(new StringFieldDefinition<T, ObjectId>("Id"), id);
            return await collection.Find(filter).SingleOrDefaultAsync();
        }
    }
}