using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using Pengqian.NetworkDisk.Infrastructure;

namespace Pengqian.NetworkDisk.DbAccess
{
    public class DbAccessUnitOfWork<T> : DbAccess<T> where T : IBaseEntity, new()
    {
        private readonly List<WriteModel<T>> _writes = new List<WriteModel<T>>();

        public override void Update(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            _writes.Add(new UpdateManyModel<T>(filter, update));
        }

        public override void Insert(params T[] source)
        {
            foreach (var item in source)
            {
                _writes.Add(new InsertOneModel<T>(item));
            }
        }

        public override void Delete(Expression<Func<T, bool>> filter)
        {
            _writes.Add(new DeleteOneModel<T>(filter));
        }

        public override void Commit()
        {
            if (_writes.Any())
                Collection.BulkWrite(_writes);
        }

        public DbAccessUnitOfWork(DbOption option) : base(option)
        {
        }

        public DbAccessUnitOfWork(MongoClient client) : base(client) { }
    }
}