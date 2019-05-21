using Microsoft.Extensions.DependencyInjection;

namespace VTV.OpsConsole.RemoteManagement.APIServer.StartupTasks
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
            where T : class, IStartupTask
            => services.AddTransient<IStartupTask, T>();
    }
}
