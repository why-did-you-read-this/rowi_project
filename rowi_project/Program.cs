using rowi_project.Data;
using rowi_project.Services;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace rowi_project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string basePath = AppContext.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", ".."));
            string envPath = Path.Combine(projectRoot, ".env");
            var builder = WebApplication.CreateBuilder(args);
            Env.Load(envPath);
            builder.Configuration.AddEnvironmentVariables();

            string host = builder.Configuration["POSTGRES_HOST"]!;
            string port = builder.Configuration["POSTGRES_PORT"]!;
            string user = builder.Configuration["POSTGRES_USER"]!;
            string password = builder.Configuration["POSTGRES_PASSWORD"]!;
            string db = builder.Configuration["POSTGRES_DB"]!;
            string connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={db}";
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            builder.Services.AddScoped<IAgentService, AgentService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
