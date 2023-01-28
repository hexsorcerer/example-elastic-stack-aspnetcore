using AutoMapper;
using Elastic.CommonSchema.Serilog;
using Serilog;
using Serilog.Events;

namespace ElasticStack.API.Services;

public class EcsTextFormatterConfigurationFactory : IEcsTextFormatterConfigurationFactory
{
    private readonly IMapper _mapper;

    public EcsTextFormatterConfigurationFactory(IMapper mapper)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public EcsTextFormatterConfiguration BuildEcsTextFormatterConfiguration(
        HostBuilderContext context,
        LoggerConfiguration config)
    {
        config.ReadFrom.Configuration(context.Configuration);
        var httpAccessor = context.Configuration.Get<HttpContextAccessor>();

        var formatterConfig = new EcsTextFormatterConfiguration();
        formatterConfig.MapHttpContext(httpAccessor);

        formatterConfig.MapCustom(MapCustomFileLogEvent);

        return formatterConfig;
    }

    private Elastic.CommonSchema.Base MapCustomFileLogEvent(Elastic.CommonSchema.Base ecsLogEvent, LogEvent logEvent)
    {
        var property = TryGetFileProperty(logEvent);
        if (property is null)
        {
            return ecsLogEvent;
        }

        ecsLogEvent.File = _mapper.Map<Elastic.CommonSchema.File>(property);
        return ecsLogEvent;
    }

    private LogEventPropertyValue? TryGetFileProperty(LogEvent logEvent)
    {
        const string className = nameof(Elastic.CommonSchema.File);
        _ = logEvent.Properties.TryGetValue(className, out var propertyValue);
        return propertyValue;
    }
}
