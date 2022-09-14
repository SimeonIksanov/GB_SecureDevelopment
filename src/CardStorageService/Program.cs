using CardStorageService.Data;
using CardStorageService.Services;
using CardStorageService.Services.Implementation;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

namespace CardStorageService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region Configure Logging service

        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
            options.RequestBodyLogLimit = 4096;
            options.ResponseBodyLogLimit = 4096;
            options.RequestHeaders.Add("Authorization");
            options.RequestHeaders.Add("X-Real-IP");
            options.RequestHeaders.Add("X-Forwarded-For");
        });

        builder.Host.ConfigureLogging(options =>
        {
            options.ClearProviders();
            options.AddConsole();
        }).UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = true });

        #endregion

        #region Configure EF DbContext Service (CardStorageService Database)
        var connection_type = builder.Configuration["Settings:DatabaseOptions:DatabaseType"];
        var connectionString = builder.Configuration["Settings:DatabaseOptions:ConnectionString"];

        switch (connection_type)
        {
            case "postgresql":
                break; // TODO: add postgresql
            case "SqlServer":
                builder.Services.AddDbContext<CardStorageServiceDbContext>(
                    options => {
                        options.UseSqlServer(connectionString, opt => opt.MigrationsAssembly("CardStorageService.Data.MsSql"));
                    });
                break;
        }


        #endregion

        #region Configure repositories

        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<ICardRepository, CardRepository>();
        #endregion
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

        app.UseHttpLogging();

        app.MapControllers();

        app.Run();
    }
}