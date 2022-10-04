using CardStorageService.Data;
using CardStorageService.Mappings;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Validators;
using CardStorageService.Services;
using CardStorageService.Services.Implementation;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.Net;
using System.Text;

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
        #region Configure Services

        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

        #endregion

        #region Configure JWT

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
		.AddJwtBearer(options =>
		{
			options.RequireHttpsMetadata = false;
			options.SaveToken = false;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Settings:AuthenticationService:SecretKey"])),
				ValidateIssuer = false,
                ValidateAudience = false,
				ClockSkew = TimeSpan.Zero,
			};
		});

        #endregion

        #region Configure AuthenticationService

        builder.Services.Configure<AuthenticationServiceConfiguration>(builder.Configuration.GetSection("Settings:AuthenticationService"));

        #endregion

        #region Configure FluentValidators

        builder.Services.AddScoped<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();
        builder.Services.AddScoped<IValidator<ClientCreateRequest>, ClientCreateRequestValidator>();
        builder.Services.AddScoped<IValidator<CardCreateRequest>, CardCreateRequestValidator>();

        #endregion

        #region Configure Mapper

        builder.Services.AddAutoMapper(typeof(MappingsProfile));

        #endregion

        #region Configure gRPC

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 5001, listenOptions =>
            {
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
            });
        });
        builder.Services.AddGrpc();

        #endregion

        builder.Services.AddMemoryCache();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CardStorageService",
                Version = "v1"
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                BearerFormat = "Bearer XXX..X",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization",
                Scheme = "Bearer",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseWhen(context => context.Request.ContentType != "application/grpc", builder => builder.UseHttpLogging());

        app.UseEndpoints(endpoint =>
        {
            endpoint.MapGrpcService<ClientService>();
            endpoint.MapGrpcService<CardService>();
        });

        app.MapControllers();

        app.Run();
    }
}