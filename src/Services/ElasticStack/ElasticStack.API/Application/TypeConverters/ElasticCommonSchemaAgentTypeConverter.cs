using AutoMapper;
using Elastic.CommonSchema;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace ElasticStack.API.Application.TypeConverters;

public class ElasticCommonSchemaAgentTypeConverter : ITypeConverter<LogEventPropertyValue, Agent?>
{
    public Agent? Convert(LogEventPropertyValue source, Agent? destination, ResolutionContext context)
    {
        var json = new StringWriter();
        new JsonValueFormatter().Format(source, json);
        Agent? result;
        try
        {
            result = JsonConvert.DeserializeObject<Agent>(json.ToString());
        }
        catch
        {
            // we return null to avoid creating an empty property on the log event
            return null;
        }

        return result;
    }
}
