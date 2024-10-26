using CineScore.Configuration;
using CineScore.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("OMDBConf.json", optional: false, reloadOnChange: true);
builder.Services.Configure<OMDBConf>(builder.Configuration);
builder.Services.AddHttpClient<OMDBService>();


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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Log.Logger = new LoggerConfiguration()
                .Enrich.WithEnvironmentUserName()
                .MinimumLevel.Debug()
            .WriteTo.Console(
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{EnvironmentUserName}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/cinescore.txt",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{EnvironmentUserName}] {Message:lj}{NewLine}{Exception}", 
                fileSizeLimitBytes: 5242880, 
                rollingInterval: RollingInterval.Day, 
                rollOnFileSizeLimit:true)
            .CreateLogger();

Log.Information("App has started");

app.Run();