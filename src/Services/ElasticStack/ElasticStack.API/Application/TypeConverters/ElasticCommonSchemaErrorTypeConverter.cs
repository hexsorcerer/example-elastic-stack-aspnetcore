using AutoMapper;
using Elastic.CommonSchema;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace ElasticStack.API.Application.TypeConverters;

public class ElasticCommonSchemaErrorTypeConverter : ITypeConverter<LogEventPropertyValue, Error?>
{
    public Error? Convert(LogEventPropertyValue source, Error? destination, ResolutionContext context)
    {
        var json = new StringWriter();
        new JsonValueFormatter().Format(source, json);
        Error? result;
        try
        {
            result = JsonConvert.DeserializeObject<Error>(json.ToString());
        }
        catch
        {
            // we return null to avoid creating an empty property on the log event
            return null;
        }

        return result;
    }
}
