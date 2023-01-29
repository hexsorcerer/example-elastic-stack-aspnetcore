using AutoMapper;
using Elastic.CommonSchema;
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

        formatterConfig.MapCustom((ecsLogEvent, logEvent) =>
        {
            MapCustomAgentLogEvent(ecsLogEvent, logEvent);
            MapCustomErrorLogEvent(ecsLogEvent, logEvent);
            MapCustomFileLogEvent(ecsLogEvent, logEvent);
            return ecsLogEvent;
        });

        return formatterConfig;
    }

    private void MapCustomAgentLogEvent(Base ecsLogEvent, LogEvent logEvent)
    {
        var property = TryGetAgentProperty(logEvent);
        if (property is null)
        {
            return;
        }

        var agentProperty = _mapper.Map<Agent>(property);
        if (agentProperty is null)
        {
            return;
        }

        ecsLogEvent.Agent = agentProperty;
    }

    private LogEventPropertyValue? TryGetAgentProperty(LogEvent logEvent)
    {
        const string className = nameof(Agent);
        _ = logEvent.Properties.TryGetValue(className, out var propertyValue);
        return propertyValue;
    }

    private void MapCustomErrorLogEvent(Base ecsLogEvent, LogEvent logEvent)
    {
        var property = TryGetErrorProperty(logEvent);
        if (property is null)
        {
            return;
        }

        var errorProperty = _mapper.Map<Error>(property);
        if (errorProperty is null)
        {
            return;
        }

        ecsLogEvent.Error = errorProperty;
    }

    private LogEventPropertyValue? TryGetErrorProperty(LogEvent logEvent)
    {
        const string className = nameof(Error);
        _ = logEvent.Properties.TryGetValue(className, out var propertyValue);
        return propertyValue;
    }

    private void MapCustomFileLogEvent(Base ecsLogEvent, LogEvent logEvent)
    {
        var property = TryGetFileProperty(logEvent);
        if (property is null)
        {
            return;
        }

        var fileProperty = _mapper.Map<Elastic.CommonSchema.File>(property);
        if (fileProperty is null)
        {
            return;
        }

        ecsLogEvent.File = fileProperty;
    }

    private LogEventPropertyValue? TryGetFileProperty(LogEvent logEvent)
    {
        const string className = nameof(Elastic.CommonSchema.File);
        _ = logEvent.Properties.TryGetValue(className, out var propertyValue);
        return propertyValue;
    }
}
