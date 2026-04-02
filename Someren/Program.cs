using Someren.Models;

namespace Someren;

using Someren.Repositories;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var program = new Program();
        program.ConfigureServices(builder.Services);
        var app = builder.Build();
        program.ConfigurePipeline(app);
        app.Run();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<ILecturerRepository, LecturerRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IActivitySupervisorRepository, ActivitySupervisorRepository>();
    }

    private void ConfigurePipeline(WebApplication app)
    {
        UseProductionSettings(app);
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        MapRoutes(app);
    }

    private void UseProductionSettings(WebApplication app)
    {
        if (app.Environment.IsDevelopment()) return;
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    private void MapRoutes(WebApplication app)
    {
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }
}