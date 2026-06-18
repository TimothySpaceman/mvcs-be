using Lib.Modules.Tasks.Configurations;
using Lib.Modules.Tasks.Repositories;
using Lib.Modules.Tasks.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Modules.Tasks;

public static class TasksModule
{
    public static IServiceCollection AddTasksModule(this IServiceCollection services)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskService, TaskService>();
        
        return services;
    }

    public static void ApplyTasksConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProjectTaskConfiguration());
        modelBuilder.ApplyConfiguration(new TaskAssignmentConfiguration());
    }
}