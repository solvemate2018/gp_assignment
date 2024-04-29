using System.Diagnostics;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PatientService.Data;
using PatientService.External;
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


builder.Services.AddDbContext<PatientDbContext>(options =>
{
    options.UseSqlServer("Server=patientsDb;Database=patientsDb;User Id=SA;Password=yourStrong(!)Password;Encrypt=no");
});

builder.Services.AddScoped<PatientService.Services.PatientService>();
builder.Services.AddScoped<MeasurementServiceCommunicator>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAllOrigins");
app.Run();