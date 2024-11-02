using CineScore.Configuration;
using CineScore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Context;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("OMDBConf.json", optional: false, reloadOnChange: true);
builder.Services.Configure<OmdbConf>(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001"; 
        options.Audience = "api1";
    });
builder.Services.AddAuthorization();

builder.Services.AddHttpClient<IOmdbService, OmdbService>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Cine Score", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var connectionString = @"Server=(localdb)\mssqllocaldb;Database=LoggingDB;Trusted_Connection=True;";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{Machine}/{Username}] {Message:lj}")
    .WriteTo.File("logs/cinescore.txt",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{Machine}/{Username}] {Message:lj}{NewLine}{Exception}",
        fileSizeLimitBytes: 5242880,
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true)
    .WriteTo.MSSqlServer(
        connectionString: connectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "CineScoreLogs",
            AutoCreateSqlTable = true,
        },
        columnOptions: new ColumnOptions
        {
            AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn
                {ColumnName = "EnvironmentUserName", PropertyName = "Username" },

                 new SqlColumn
                {ColumnName = "MachineName", PropertyName = "Machine" }
            }
        })            
    .CreateLogger();

using (LogContext.PushProperty("Username", Environment.UserName))
using (LogContext.PushProperty($"Machine", Environment.MachineName))
    Log.Information("App has started");

app.Run();
