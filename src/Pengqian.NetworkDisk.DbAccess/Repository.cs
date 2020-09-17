using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using Pengqian.NetworkDisk.Infrastructure;

namespace Pengqian.NetworkDisk.DbAccess
{
    public class Repository<T> : IRepository<T> where T : IBaseEntity, new()
    {
        public IMongoClient Client { get; private set; }
        public DbOption DbOption { get; }
        public bool CanSubmit { get; private set; }
        public IMongoCollection<T> Collection => _access.Collection;
        public IMongoQueryable<T> MongodbQueryable => _access.MongoQueryable;

        private DbAccess<T> _access;

        public Repository()
        {
            DbOption = DbTools.DefaultOption;
            Init(DbOption);
        }

        public Repository(DbOption option)
        {
            DbOption = option;
            Init(option);
        }

        void Init(DbOption option)
        {
            CanSubmit = false;
            _access = new DbAccessEntity<T>(option);
            Client = _access.Client;
        }

        public void Insert(params T[] source)
        {
            _access.Insert(source);
        }

        public void Delete(Expression<Func<T, bool>> filter)
        {
            _access.Delete(filter);
        }

        public void Delete(T t)
        {
            _access.Collection.DeleteOne(new ObjectFilterDefinition<T>(t));
        }

        public void Update(Expression<Func<T, bool>> predicate, UpdateDefinition<T> update)
        {
            _access.Update(predicate, update);
        }

        public void Update(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new ArgumentException("In the entity no exists property 'id'.", nameof(entity));
            var id = idProperty.GetValue(entity);
            if (id == null)
                throw new ArgumentException("The entity property 'id' value is null.", nameof(entity));
            //var idTypeName = idProperty.PropertyType.Name;
            var param = Expression.Parameter(typeof(T), "__q");
            var left = Expression.Property(param, idProperty);
            var right = Expression.Constant(id);
            var equalExpression = (Expression) Expression.Equal(left, right);
            var lambda = Expression.Lambda<Func<T, bool>>(equalExpression, param);
            FilterDefinition<T> filter = lambda;
            Collection.ReplaceOne(filter, entity);
        }

        public IMongoQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return _access.MongoQueryable.Where(predicate);
        }

        public T Find(ObjectId id)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null && idProperty.PropertyType == typeof(ObjectId))
                return Collection.Find(id);
            return default(T);
        }

        public void Commit()
        {
            _access.Commit();
        }

        public async Task<T> FindAsync(ObjectId id)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null && idProperty.PropertyType == typeof(ObjectId))
                return await Collection.FindAsync(id);
            return default(T);
        }

        public async Task InsertAsync(params T[] source)
        {
            await Collection.InsertManyAsync(source);
        }

        public async Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> filter)
        {
            var result = await Collection.DeleteManyAsync(filter);
            return result;
        }

        public async Task<UpdateResult> UpdateAsync(Expression<Func<T, bool>> predicate, UpdateDefinition<T> update)
        {
            var result = await Collection.UpdateManyAsync(predicate, update);
            return result;
        }

        public async Task<ReplaceOneResult> UpdateAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new ArgumentException("In the entity no exists property 'id'.", nameof(entity));
            var id = idProperty.GetValue(entity);
            if (id == null)
                throw new ArgumentException("The entity property 'id' value is null.", nameof(entity));
            var idTypeName = idProperty.PropertyType.Name;
            FilterDefinition<T> filter;
            switch (idTypeName)
            {
                case "ObjectId":
                    var definitionObjectId = new StringFieldDefinition<T, ObjectId>("Id");
                    filter = Builders<T>.Filter.Eq(definitionObjectId, (ObjectId) id);
                    break;
                case "Int32":
                    var definitionInt32 = new StringFieldDefinition<T, int>("Id");
                    filter = Builders<T>.Filter.Eq(definitionInt32, (int) id);
                    break;
                case "String":
                    var definitionString = new StringFieldDefinition<T, string>("Id");
                    filter = Builders<T>.Filter.Eq(definitionString, (string) id);
                    break;
                default:
                    throw new Exception($"Do not support {idTypeName} type!");
            }

            var result = await Collection.ReplaceOneAsync(filter, entity);
            return result;
        }

        public async Task<bool> IsExists(string md5)
        {
            var gridFs = new GridFSBucket(_access.Database);
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.MD5, md5);
            var result = await gridFs.FindAsync(filter);
            while (await result.MoveNextAsync())
            {
                if (result.Current.Any())
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<string> FindFileId(string md5)
        {
            var gridFs = new GridFSBucket(_access.Database);
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.MD5, md5);
            var result = await gridFs.FindAsync(filter);
            while (await result.MoveNextAsync())
            {
                foreach (var item in result.Current)
                {
                    return item.Id.ToString();
                }
            }

            return "";
        }

        public async Task<string> UploadAsync(string fileName, Stream stream)
        {
            var gridFs = new GridFSBucket(_access.Database);
            var id = await gridFs.UploadFromStreamAsync(fileName, stream);
            return id.ToString();
        }

        public async Task<MongodbFileResult> DownloadAsync(string id)
        {
            if (ObjectId.TryParse(id, out var oid))
            {
                try
                {
                    var gridFs = new GridFSBucket(_access.Database);
                    //var result = gridFs.Find(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Id, id)).FirstOrDefault();
                    var result = await gridFs.OpenDownloadStreamAsync(oid);
                    var fileResult = new MongodbFileResult
                        {FileName = result.FileInfo.Filename, Stream = result, Md5 = result.FileInfo.MD5};
                    return fileResult;
                }
                catch (GridFSFileNotFoundException)
                {
                    return null;
                }
            }

            return null;
        }

        public async Task DeleteAsync(string id)
        {
            if (ObjectId.TryParse(id, out var oid))
            {
                var gridFs = new GridFSBucket(_access.Database);
                //var result = gridFs.Find(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Id, id)).FirstOrDefault();
                try
                {
                    await gridFs.DeleteAsync(oid);
                }
                catch (GridFSFileNotFoundException)
                {
                }
            }
        }

        public IEnumerable<string> TodayData()
        {
            var gridFs = new GridFSBucket(_access.Database);
            var time = DateTime.Now.Date;
            FilterDefinition<GridFSFileInfo> filter =
                new ExpressionFilterDefinition<GridFSFileInfo>(x => x.UploadDateTime > time);
            var result = gridFs.Find(filter);
            while (result.MoveNext())
            {
                foreach (var item in result.Current)
                {
                    yield return item.Id.ToString();
                }
            }
        }
    }
}