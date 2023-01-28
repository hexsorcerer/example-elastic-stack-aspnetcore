using AutoMapper;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting.Json;
using File = Elastic.CommonSchema.File;

namespace ElasticStack.API.Application.TypeConverters;

public class ElasticCommonSchemaFileTypeConverter : ITypeConverter<LogEventPropertyValue, File?>
{
    public File? Convert(LogEventPropertyValue source, File? destination, ResolutionContext context)
    {
        var json = new StringWriter();
        new JsonValueFormatter().Format(source, json);
        File? result;
        try
        {
            result = JsonConvert.DeserializeObject<File>(json.ToString());
        }
        catch
        {
            // we return null to avoid creating an empty property on the log event
            return null;
        }

        return result;
    }
}
