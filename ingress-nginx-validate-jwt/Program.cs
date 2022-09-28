using Microsoft.IdentityModel.Tokens;

namespace ingress_nginx_validate_jwt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddSingleton<SettingsService>();

            builder.Services.AddHostedService<HostedService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseForwardedHeaders();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}