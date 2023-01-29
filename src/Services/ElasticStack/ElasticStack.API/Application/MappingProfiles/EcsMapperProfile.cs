using AutoMapper;
using ElasticStack.API.Application.TypeConverters;
using Serilog.Events;

namespace ElasticStack.API.Application.MappingProfiles;

public class EcsMapperProfile : Profile
{
    // null values from the custom type converters are used in the factory to
    // avoid putting empty objects on the log event
    public EcsMapperProfile()
    {
        CreateMap<LogEventPropertyValue, Elastic.CommonSchema.Agent>()
            .ConvertUsing(new ElasticCommonSchemaAgentTypeConverter());

        CreateMap<LogEventPropertyValue, Elastic.CommonSchema.Error>()
            .ConvertUsing(new ElasticCommonSchemaErrorTypeConverter());

        CreateMap<LogEventPropertyValue, Elastic.CommonSchema.File>()
            .ConvertUsing(new ElasticCommonSchemaFileTypeConverter());
    }
}
