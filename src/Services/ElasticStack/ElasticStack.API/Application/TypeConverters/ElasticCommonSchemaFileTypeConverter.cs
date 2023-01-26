using AutoMapper;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting.Json;
using File = Elastic.CommonSchema.File;

namespace ElasticStack.API.Application.TypeConverters;

public class ElasticCommonSchemaFileTypeConverter : ITypeConverter<LogEventPropertyValue, File>
{
    public File Convert(LogEventPropertyValue source, File destination, ResolutionContext context)
    {
        var json = new StringWriter();
        new JsonValueFormatter().Format(source, json);
        var result = JsonConvert.DeserializeObject<File>(json.ToString());
        return result ?? new File();
    }
}
