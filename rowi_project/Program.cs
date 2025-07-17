using Microsoft.EntityFrameworkCore;
using rowi_project.Data;
using rowi_project.Mapping;
using rowi_project.Middleware;

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
                                    "http://localhost:13022"
                                )
                                .AllowAnyHeader()
                                .AllowAnyMethod());
        });
        //builder.Services.AddScoped<IAgentService, AgentService>();
        //builder.Services.AddScoped<IBankService, BankService>();

        builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
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
