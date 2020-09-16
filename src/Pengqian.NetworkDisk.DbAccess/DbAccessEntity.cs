using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Pengqian.NetworkDisk.Infrastructure;
using UpdateResult = MongoDB.Driver.UpdateResult;

namespace Pengqian.NetworkDisk.DbAccess
{
    public class DbAccessEntity<T> : DbAccess<T> where T : IBaseEntity, new()
    {
        public override void Update(Expression<Func<T, bool>> filter, MongoDB.Driver.UpdateDefinition<T> update)
        {
            Collection.UpdateMany(filter, update);
        }

        public override void Insert(params T[] source)
        {
            Collection.InsertMany(source);
        }

        public override void Delete(Expression<Func<T, bool>> filter)
        {
            Collection.DeleteMany(filter);
        }

        public override void Commit()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修改符合条件的所有数据
        /// </summary>
        /// <param name="updateContent">要更改的内容，key为属性名，value为要更改的值</param>
        /// <param name="expr"></param>
        public async Task<UpdateResult> UpdateAsync(Dictionary<string, object> updateContent,
            Expression<Func<T, bool>> expr)
        {
            var update = new BsonDocument("$set", new BsonDocument(updateContent));
            var result = await Collection.UpdateManyAsync(expr, update);
            return result;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task InsertManyAsync(IEnumerable<T> source)
        {
            await Collection.InsertManyAsync(source);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task InsertAsync(T entity)
        {
            await Collection.InsertOneAsync(entity);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="isMore">是否根据筛选结果删除多条数据 True:删除多条 False:删除1条</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> filter,
            bool isMore)
        {
            DeleteResult result;
            if (isMore)
                result = await Collection.DeleteManyAsync(filter);
            else
                result = await Collection.DeleteOneAsync(filter);
            return result;
        }

        public DbAccessEntity(DbOption option) : base(option)
        {
        }

        public DbAccessEntity(IMongoClient client) : base(client)
        {
        }
    }
}