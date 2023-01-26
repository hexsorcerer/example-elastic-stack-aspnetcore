using Elastic.CommonSchema.Serilog;
using Serilog;

namespace ElasticStack.API.Services;

public interface IEcsTextFormatterConfigurationFactory
{
    EcsTextFormatterConfiguration BuildEcsTextFormatterConfiguration(
        HostBuilderContext context,
        LoggerConfiguration config);
}
