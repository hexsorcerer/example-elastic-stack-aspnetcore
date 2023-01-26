using AutoMapper;
using Elastic.CommonSchema.Serilog;
using ElasticStack.API.Application.TypeConverters;
using Serilog;
using Serilog.Events;

namespace ElasticStack.API.Services;

public class EcsTextFormatterConfigurationFactory : IEcsTextFormatterConfigurationFactory
{
    public EcsTextFormatterConfiguration BuildEcsTextFormatterConfiguration(
        HostBuilderContext context,
        LoggerConfiguration config)
    {
        config.ReadFrom.Configuration(context.Configuration);
        var httpAccessor = context.Configuration.Get<HttpContextAccessor>();

        var formatterConfig = new EcsTextFormatterConfiguration();
        formatterConfig.MapHttpContext(httpAccessor);

        var mapConfig = new MapperConfiguration(cfg =>
            cfg.CreateMap<LogEventPropertyValue, Elastic.CommonSchema.File>()
                .ConvertUsing(new ElasticCommonSchemaFileTypeConverter()));

        var mapper = mapConfig.CreateMapper();

        formatterConfig.MapCustom((ecsLogEvent, logEvent) =>
        {
            const string className = nameof(Elastic.CommonSchema.File);
            var success = logEvent.Properties.TryGetValue(className, out var propertyValue);
            if (!success || propertyValue is null)
            {
                return ecsLogEvent;
            }

            ecsLogEvent.File = mapper.Map<Elastic.CommonSchema.File>(propertyValue);
            return ecsLogEvent;
        });

        return formatterConfig;
    }
}
