
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Middleware;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.AuthService;
using PitchSwitchBackend.Services.ClubService;
using PitchSwitchBackend.Services.CommentService;
using PitchSwitchBackend.Services.DeleteExpiredTokensJob;
using PitchSwitchBackend.Services.ImageService;
using PitchSwitchBackend.Services.JobExecutor;
using PitchSwitchBackend.Services.JournalistStatusApplicationService;
using PitchSwitchBackend.Services.PlayerService;
using PitchSwitchBackend.Services.PostService;
using PitchSwitchBackend.Services.TokenService;
using PitchSwitchBackend.Services.TransferRumourService;
using PitchSwitchBackend.Services.TransferService;

namespace PitchSwitchBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddHangfire(config =>
            {
                config.UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddHangfireServer();

            builder.Services.AddScoped<IDeleteExpiredTokensJobService, DeleteExpiredTokensJobService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            builder.Services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 12;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IClubService, ClubService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<ITransferService, TransferService>();
            builder.Services.AddScoped<ITransferRumourService, TransferRumourService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IJournalistStatusApplicationService, JournalistStatusApplicationService>();
            builder.Services.AddSingleton<IJobExecutor, JobExecutor>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard(options: new DashboardOptions
            {
                DisplayStorageConnectionString = false
            });

            var jobExecutor = app.Services.GetRequiredService<IJobExecutor>();
            RecurringJob.AddOrUpdate("CleanExpiredRefreshTokens", () => jobExecutor.CleanExpiredTokensJob(), Cron.Daily);

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseStaticFiles();

            app.MapControllers();
            
            app.Run();
        }
    }
}
