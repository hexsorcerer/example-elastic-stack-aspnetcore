using System.Globalization;
using System.Reflection;
using Serilog;
using Serilog.Formatting.Json;

var configuration = GetConfiguration();
Log.Logger = CreateSerilogLogger(configuration);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

IConfiguration GetConfiguration()
{
    return new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();
}

Serilog.ILogger CreateSerilogLogger(IConfiguration config)
{
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", Assembly.GetExecutingAssembly().GetName().Name)
        .Enrich.FromLogContext()
        .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
        .WriteTo.Http(
            requestUri: "http://logstash:5000",
            queueLimitBytes: null,
            textFormatter: new JsonFormatter(),
            configuration: config)
        .CreateLogger();
}
