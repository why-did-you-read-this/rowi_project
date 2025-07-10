using Microsoft.EntityFrameworkCore;
using rowi_project.Data;
using rowi_project.Mapping;
using rowi_project.Middleware;
using rowi_project.Services;

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

        builder.Services.AddAutoMapper(typeof(MappingProfile));

        var app = builder.Build();
        
        app.UseMiddleware<ExceptionHandlingMiddleware>();

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
