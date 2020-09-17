using System;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Bson;
using Pengqian.NetworkDisk.DbAccess;
using Pengqian.NetworkDisk.Infrastructure;

namespace Pengqian.NetworkDisk.Service
{
    internal static class Share
    {
        internal static DbOption DbOption = null;
    }
    
    public abstract class BasicService<T> where T : IBaseEntity, new()
    {
        /// <summary>
        /// Db 配置
        /// </summary>
        protected DbOption DbOption => Share.DbOption;
        
        /// <summary>
        /// Repository 实例
        /// </summary>
        protected IRepository<T> Repository { get; }
        
        protected IMapper Mapper => SingleMapper.GetInstance().Mapper;

        /// <summary>
        /// 默认 Repository
        /// </summary>
        protected BasicService()
        {
            Share.DbOption ??= DbTools.DefaultOption;
            Repository = new Repository<T>(Share.DbOption);
        }

        /// <summary>
        /// 指定 Db 配置，创建 Repository 实例
        /// </summary>
        /// <param name="option"></param>
        protected BasicService(DbOption option)
        {
            Share.DbOption ??= option;
            Repository = new Repository<T>(Share.DbOption);
        }

        /// <summary>
        /// 根据string类型的ID查询实体
        /// </summary>
        protected Func<string, Task<T>> GetEntityByStringId => async id =>
        {
            if (ObjectId.TryParse(id, out var oid))
            {
                return await Repository.FindAsync(oid);
            }

            return default;
        };

        /// <summary>
        /// 从<see cref="string"/>转换成<see cref="ObjectId"/>,如果成功，将异步执行 <see cref="Action"/>
        /// </summary>
        /// <param name="strId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected async Task ToObjectIdAction(string strId, Func<ObjectId, IRepository<T>, Task> action)
        {
            if (ObjectId.TryParse(strId, out var id))
            {
                await action(id, Repository);
            }
        }
    }
}