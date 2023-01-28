using AutoMapper;
using ElasticStack.API.Application.TypeConverters;
using Serilog.Events;

namespace ElasticStack.API.Application.MappingProfiles;

public class EcsMapperProfile : Profile
{
    public EcsMapperProfile()
    {
        CreateMap<LogEventPropertyValue, Elastic.CommonSchema.File>()
            .ConvertUsing(new ElasticCommonSchemaFileTypeConverter());
    }
}
