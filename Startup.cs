using KtsIndexBackend.Databases;
using KtsIndexBackend.Interfaces;
using KtsIndexBackend.Repositories;
using KtsIndexBackend.Services;
using Microsoft.OpenApi.Models;

namespace KtsIndexBackend;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "kts-index API", Version = "v1" });
            c.AddSecurityDefinition(
                "ApiKey",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description =
                        "API Key needed to access the endpoints. API Key must be in the 'Authorization' header. For public endpoints you can didn't use API Key, or just type '0'",
                    Name = "Authorization"
                }
            );
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" } }, [] }
            });
        });

        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        services.AddSingleton<IMongoDbContext, MongoDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthorizeService, AuthorizeService>();
        services.AddScoped<IUserService, UserService>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", builder =>
            {
                builder
                    .SetIsOriginAllowed(origin =>
                    {
                        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;
                        return uri.Host.EndsWith("mzh.su") || uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase);
                    })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });

        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseHsts();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors("AllowSpecificOrigins");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute().AllowAnonymous();
            endpoints.MapSwagger();
            endpoints.MapControllers().AllowAnonymous();
        });
    }
}