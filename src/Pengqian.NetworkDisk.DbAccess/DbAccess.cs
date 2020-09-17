using System;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pengqian.NetworkDisk.Infrastructure;

namespace Pengqian.NetworkDisk.DbAccess
{
    public abstract  class DbAccess<T> where T : IBaseEntity, new()
    {
        private readonly IMongoClient _client;

        private readonly string _collectionName;
        
        private readonly string _dataBaseName;
        
        protected DbAccess(DbOption option)
        {
            if (string.IsNullOrEmpty(option.Db))
            {
                throw new ArgumentException("Value cannot be empty.",nameof(option.Db));
            }
            if (string.IsNullOrEmpty(option.Host))
            {
                throw new ArgumentException("Value cannot be empty.",nameof(option.Host));
            }
            _dataBaseName = option.Db;
            var setting = new MongoClientSettings
            {
                Server = new MongoServerAddress(option.Host, option.Port)
            };
            if (!string.IsNullOrEmpty(option.ConnUser) && !string.IsNullOrEmpty(option.Password))
            {
                MongoIdentity identity = new MongoExternalIdentity(option.Db, option.ConnUser);
                MongoIdentityEvidence identityEvidence = new PasswordEvidence(option.Password);
                setting.Credential = new MongoCredential("SCRAM-SHA-1", identity, identityEvidence);
            }
            _client = new MongoClient(setting);
            _collectionName = typeof(T).Name;
        }

        protected DbAccess(IMongoClient client)
        {
            _dataBaseName = client.Settings.Credential.Identity.Source;
            _client = client;
            _collectionName = typeof(T).Name;
        }
        
        /// <summary>
        /// 获取Mongodb客户端
        /// </summary>
        internal virtual IMongoClient Client => _client;

        /// <summary>
        /// 获取 MongoDB 数据库
        /// </summary>
        public virtual IMongoDatabase Database => _client.GetDatabase(_dataBaseName);

        /// <summary>
        /// 获取 该实体中 MongoDB数据库的集合
        /// </summary>
        public virtual IMongoCollection<T> Collection => Database.GetCollection<T>(_collectionName);

        /// <summary>
        /// 获取 提供对MongoDB数据查询的Queryable
        /// </summary>
        /// <returns></returns>
        public IMongoQueryable<T> MongoQueryable => Database.GetCollection<T>(_collectionName).AsQueryable(
            new AggregateOptions {AllowDiskUse = true});

        /// <summary>
        /// 根据筛选条件更新数据
        /// </summary>
        /// <param name="filter">筛选条件</param>
        /// <param name="update">要修改的数据</param>
        public abstract void Update(Expression<Func<T, bool>> filter, UpdateDefinition<T> update);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="source"></param>
        public abstract void Insert(params T[] source);

        /// <summary>
        /// 根据筛选条件删除数据
        /// </summary>
        /// <param name="filter"></param>
        public abstract void Delete(Expression<Func<T, bool>> filter);


        public virtual void StartSession()
        {
            using (var session = _client.StartSession())
            {
            }
        }

        public abstract void Commit();
    }
}