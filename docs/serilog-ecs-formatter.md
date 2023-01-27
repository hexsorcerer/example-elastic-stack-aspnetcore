# Serilog EcsTextFormatter Configuration
Getting this configured just right was the key to making this work, and it was
super tricky to figure out.

## Get the HttpContextAccessor to populate the user field
### Why do we need this?
The ECS defines a field named
[```user```](https://www.elastic.co/guide/en/ecs/current/ecs-user.html)
that is intended to hold information about a user. You might be tempted to use
this field to store information about a user in your application, but one of
the contributors to the project
[pointed out](https://github.com/elastic/ecs-dotnet/issues/133#issuecomment-776322385)
that this particular user field is intended to contain information about the
user making the request.

### How to get this
So, in order to populate that field as the schema intended, it needs to be
mapped into the formatter configuration, like so:
```
config.ReadFrom.Configuration(context.Configuration);
var httpAccessor = context.Configuration.Get<HttpContextAccessor>();

var formatterConfig = new EcsTextFormatterConfiguration();
formatterConfig.MapHttpContext(httpAccessor);
```

The ```config``` and ```context``` are available to you in the lambda used to
configure Serilog:
```
builder.Host.UseSerilog((context, config) => { ... }
```

## Mapping LogEventPropertyValues to ECS types
If there was any magic to be found, this is it right here. Big thanks to github
user sgtobin for providing
[the original solution](https://github.com/elastic/ecs-dotnet/issues/133#issuecomment-777576172).

Let's say you wanted to add the ECS ```file``` field to a log message. Here's
the flow of events that makes that possible:

First, create your ECS ```File``` object and populate whatever fields you want
on it:
```
var file = new Elastic.CommonSchema.File
{
    Path = "/home/me/mystuff",
    Name = "somefile",
    Type = new Random().Next(1, 101) % 2 == 0 ? "txt" : "pdf"
};

_logger.LogInformation("{@File}", file);
```
The ```@``` notation indicates file is a complex object and not just a string,
so it will populate the LogEventPropertyValue as a JSON string.

Now you need to intercept that property in the ECS text formatter configuration
and do a custom map of the JSON value from Serilog back into an ECS object:
```
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
```
Here, we're specifically looking for a LogEventPropertyValue with a key of
```File``` because that's the structured log event name we logged to Serilog.

The mapping of the property value back into an ECS object could be done in-line,
but I chose to make use of AutoMapper to do a custom map:
```
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
```
This converts the LogEventPropertyValue to JSON, which we then deserialize back
into an ECS object.

## Cleanup
Now, you might be satisifed at this point, but there was one more thing I wanted
to do to make this as perfect as I could. Now that we have our ECS-approved
```file``` field populated, we're also going to have all of the ```file``` data,
including all the null fields, nested in another field called ```_metadata```.
This is where the ECS allows you to place all of your custom structured data
fields, and anything that's not specifically mapped elsewhere will end up here.

So how do we clean up our duplicate ```_metadata.file``` field? Logstash mutate
filter:
```
filter {
    mutate {
        remove_field => [ "[metadata][file]" ]
    }
}
```
Now your log message should have the ECS ```file``` field and no duplicate data
under ```_metadata```.

## Is there a simpler way?
Not that I could find, and I looked for several days. This was the only thing
that worked. I will be the first to admit that this feels super-hackity and a
lot of work for something relatively simple. Hopefully the ```ecs-dotnet``` team
will come up with some improvements in this process, but for now this seems like
the best we can do.
