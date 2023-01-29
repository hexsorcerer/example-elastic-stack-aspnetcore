using Microsoft.AspNetCore.Mvc;

namespace ElasticStack.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var agent = new Elastic.CommonSchema.Agent
        {
            EphemeralId = "8a4f500f",
            Id = "8a4f500d",
            Name = "foo",
            Type = "filebeat",
            Version = "6.0.0-rc2"
        };

        var error = new Elastic.CommonSchema.Error
        {
            Code = "666",
            Id = Guid.NewGuid().ToString(),
            Message = "Guru Meditation",
            StackTrace = "Bad stuff happened here => and here => and here",
            Type = "StackOverflow"
        };

        var file = new Elastic.CommonSchema.File
        {
            Path = "/home/me/mystuff",
            Name = "somefile",
            // provides some variable data to see in the kibana dashboard
            Type = new Random().Next(1, 101) % 2 == 0 ? "txt" : "pdf"
        };

        _logger.LogInformation("{@Agent}{@Error}{@File}", agent, error, file);
        _logger.LogInformation(
            "This message has no {ElasticCommonSchema} fields so it will appear in the console too",
            nameof(Elastic.CommonSchema));

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
