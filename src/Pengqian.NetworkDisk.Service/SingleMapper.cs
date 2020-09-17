using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;
using MongoDB.Bson;
using Pengqian.NetworkDisk.Infrastructure;

namespace Pengqian.NetworkDisk.Service
{
    public class SingleMapper
    {
        private static SingleMapper _instance;

        private static readonly object Obj = new object();

        private SingleMapper()
        {
            Mapper = ConfigGlobalMapper();
        }

        public IMapper Mapper { get; }

        public static SingleMapper GetInstance()
        {
            lock (Obj)
            {
                return _instance ??= new SingleMapper();
            }
        }
        
        private IMapper ConfigGlobalMapper()
        {
            var types = Assembly.Load("Pengqian.NetworkDisk.Public.ViewModel").GetExportedTypes();
            var config = new MapperConfigurationExpression();

            // 默认规则Mapper
            var maps = from x in types
                from y in x.GetInterfaces()
                let faceType = y.GetTypeInfo()
                where faceType.IsGenericType && y.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                      !x.IsAbstract && !x.IsInterface
                select new
                {
                    Source = y.GetGenericArguments()[0],
                    Destination = x
                };
            foreach (var item in maps)
            {
                config.CreateMap(item.Source, item.Destination);
                config.CreateMap(item.Destination, item.Source);
            }

            // 自定义Mapper
            var customMaps = from x in types
                from y in x.GetInterfaces()
                where typeof(IHaveCustomMapping).IsAssignableFrom(x) &&
                      !x.GetTypeInfo().IsAbstract &&
                      !x.GetTypeInfo().IsInterface
                select (IHaveCustomMapping) Activator.CreateInstance(x);
            foreach (var item in customMaps)
            {
                item.CreateMappings(config);
            }

            config.CreateMap<string, ObjectId>().ConstructUsing((x, y) =>
                !string.IsNullOrEmpty(x) && ObjectId.TryParse(x, out var oid) ? oid : ObjectId.Empty);
            config.CreateMap<DateTime, DateTime>().ConvertUsing((utc, local, context) =>
            {
                if (local == new DateTime())
                {
                    if (utc.Kind == DateTimeKind.Utc)
                        return utc.ToLocalTime();
                    return utc;
                }
                if (local.Kind == DateTimeKind.Local)
                    return utc.ToUniversalTime();
                return local;
            });
            var cfg = new MapperConfiguration(config);
            IMapper mapper = new Mapper(cfg);
            return mapper;
        }
    }
}