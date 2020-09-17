using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pengqian.NetworkDisk.Infrastructure;

namespace Pengqian.NetworkDisk.DbAccess
{
    public interface IRepository
    {
    }

    public interface IRepository<T> where T : IBaseEntity
    {
        /// <summary>
        /// mongodb客户端
        /// </summary>
        IMongoClient Client { get; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        DbOption DbOption { get; }

        /// <summary>
        /// 是否为工作单元
        /// </summary>
        bool CanSubmit { get; }

        /// <summary>
        /// 获取 该实体Mongodb的集合
        /// </summary>
        IMongoCollection<T> Collection { get; }

        IMongoQueryable<T> MongodbQueryable { get; }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        void Insert(params T[] source);


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        void Delete(Expression<Func<T, bool>> filter);

        /// <summary>
        /// 根据实体删除数据
        /// </summary>
        /// <param name="t"></param>
        void Delete(T t);

        /// <summary>
        /// 根据查询条件更新数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        void Update(Expression<Func<T, bool>> predicate, UpdateDefinition<T> update);

        /// <summary>
        /// 根据实体修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <exception cref="ArgumentException">If the entity no exists property 'id',then will thow exception.</exception>
        void Update(T entity);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IMongoQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据ObjectId获取单条记录，不存在将会返回null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Find(ObjectId id);

        void Commit();


        #region 异步方法

        /// <summary>
        /// (异步)根据ObjectId获取单条记录，不存在将会返回null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> FindAsync(ObjectId id);


        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        Task InsertAsync(params T[] source);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        /// 根据查询条件更新数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        Task<UpdateResult> UpdateAsync(Expression<Func<T, bool>> predicate, UpdateDefinition<T> update);

        /// <summary>
        /// 根据实体修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <exception cref="ArgumentException">If the entity no exists property 'id',then will thow exception.</exception>
        Task<ReplaceOneResult> UpdateAsync(T entity);

        #endregion

        #region Mongodb file method

        /// <summary>
        /// 根据MD5判断文件是否存在
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        Task<bool> IsExists(string md5);

        /// <summary>
        /// 根据MD5获取相应文档ID
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        Task<string> FindFileId(string md5);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<string> UploadAsync(string fileName, Stream stream);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MongodbFileResult> DownloadAsync(string id);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// 今天上传的文件
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> TodayData();

        #endregion
    }
}