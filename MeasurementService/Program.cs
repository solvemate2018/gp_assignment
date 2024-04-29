using System.Diagnostics;
using System.Reflection;
using MeasurementService.Data;
using MeasurementService.External;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serviceName = Assembly.GetCallingAssembly().GetName().Name ?? "Unknown";
var activitySource = new ActivitySource(serviceName);

var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Seq("http://seq:5341", batchPostingLimit: 10)
    .WriteTo.Console()
    .Enrich.WithSpan()
    .CreateLogger();

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddZipkinExporter(config => config.Endpoint = new Uri("http://zipkin:9411/api/v2/spans"))
    .AddConsoleExporter()
    .AddSource(activitySource.Name)
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: serviceName))
    .Build();

builder.Services.AddSingleton(activitySource);

builder.Logging.Services.AddSingleton(logger);

builder.Services.AddDbContext<MeasurementsDbContext>(options =>
{
    options.UseSqlServer("Server=measurementsDb;Database=measurementsDb;User Id=SA;Password=yourStrong(!)Password;Encrypt=no");
});

builder.Services.AddScoped<MeasurementService.Services.MeasurementService>();
builder.Services.AddScoped<PatientServiceCommunicator>();

builder.Services.AddCors(options => options.AddPolicy("AllowAllOrigins",
    builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    })
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.MapControllers();

app.UseCors("AllowAllOrigins");

app.Run();