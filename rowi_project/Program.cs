using rowi_project.Data;
using rowi_project.Services;
using Microsoft.EntityFrameworkCore;

namespace rowi_project;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string connectionString = builder.Configuration["ConnectionString"]!;
        Console.WriteLine($"connectionString: \"{connectionString}\"");

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy.WithOrigins(
                                    "http://localhost:5173",     
                                    "http://localhost:51558",    
                                    "https://localhost:7148"     
                                )
                                .AllowAnyHeader()
                                .AllowAnyMethod());
        });
        builder.Services.AddScoped<IAgentService, AgentService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors("AllowFrontend");
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
