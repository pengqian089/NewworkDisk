using AutoMapper.Configuration;

namespace Pengqian.NetworkDisk.Infrastructure
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(MapperConfigurationExpression cfg);
    }
}