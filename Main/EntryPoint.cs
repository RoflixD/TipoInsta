using Main.Defaults;
using Main.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program
{
    /// <summary>
    /// Entry point to the program! Everything starting here!
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        var app = ConfigureServices(args);
        BuildApplication(app);
    }

    /// <summary>
    /// Use this method to configure services
    /// </summary>
    /// <param name="args">Params from console</param>
    /// <returns>new web app with services</returns>
    private static WebApplication ConfigureServices(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var authSection = builder.Configuration.GetSection(AuthConfig.Position);
        var authConfig = authSection.Get<AuthConfig>();

        builder.Services.Configure<AuthConfig>(authSection);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => {
            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme {
                Description = "Enter the token!",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,
                        },
                        Scheme = "oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });
        builder.Services.AddDbContext<DAL.DataContext>(options => {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"), sql => { });
        });
        builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
        builder.Services.AddAuthentication(o => {
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o => {
            o.RequireHttpsMetadata = false;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = authConfig.Audience,
                ValidateLifetime = true,                
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = authConfig.SymmetricSecurityKey(),
                ClockSkew = TimeSpan.Zero
            };
        });
        builder.Services.AddAuthorization(o => {
            o.AddPolicy("ValidAccessToken", p => {
                p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();
            });
        });

        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<AttachService>();
        builder.Services.AddScoped<PostService>();

        return builder.Build();
    }

    /// <summary>
    /// Just build the app
    /// </summary>
    /// <param name="app">Configured Application</param>
    private static void BuildApplication(WebApplication app)
    {
        MigrateData(app);
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseTokenValidator();
        app.MapControllers();
        app.Run();
    }

    /// <summary>
    /// Apply migrations at startup
    /// you can apply it by cmd btw!
    /// </summary>
    /// <param name="app"></param>
    private static bool MigrateData(WebApplication app)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        if (app == null)
        {
            Console.WriteLine("Migration: Couldn't get web application");
            return false;
        }
        using (var svcScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
        {
            if (svcScope == null)
            {
                Console.WriteLine("Migration: Couldn't get service scope!");
                return false;
            }
            var context = svcScope.ServiceProvider.GetRequiredService<DAL.DataContext>();
            context.Database.Migrate();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Migration: succeed!");
            Console.ResetColor();
            return true;
        }
    }
}