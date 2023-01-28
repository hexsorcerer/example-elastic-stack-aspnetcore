using System.Globalization;
using System.Reflection;
using AutoMapper;
using Elastic.CommonSchema.Serilog;
using ElasticStack.API.Application.MappingProfiles;
using ElasticStack.API.Services;
using Serilog;

var configuration = GetConfiguration();
Log.Logger = CreateSerilogLogger(configuration);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper((config) =>
{
    config.AddProfile<EcsMapperProfile>();
});

builder.Host.UseSerilog((context, serviceProvider, config) =>
{
    IEcsTextFormatterConfigurationFactory factory = new EcsTextFormatterConfigurationFactory(
        serviceProvider.GetRequiredService<IMapper>());
    var formatterConfig = factory.BuildEcsTextFormatterConfiguration(context, config);
    var formatter = new EcsTextFormatter(formatterConfig);

    config.Enrich.WithProperty("ApplicationContext", Assembly.GetExecutingAssembly().GetName().Name);

    config.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);
    config.WriteTo.Http(
        requestUri: "http://logstash:5000",
        queueLimitBytes: null,
        textFormatter: formatter,
        configuration: context.Configuration);
});

// Add services to the container.
builder.Services.AddHttpContextAccessor();

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
            textFormatter: new EcsTextFormatter(),
            configuration: config)
        .CreateLogger();
}
