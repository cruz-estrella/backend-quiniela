using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace UseCases
{
    public static class UseCasesDependencyInjection
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services, string encriptionKey)
        {
            var assembly = typeof(UseCasesDependencyInjection).Assembly;

            RegisterUseCaseRecords(services, assembly);
            RegisterUseCaseHandlers(services, assembly);
            services.AddSingleton(new EncryptionHelper(encriptionKey));

            return services;
        }

        private static void RegisterUseCaseRecords(IServiceCollection services, Assembly assembly)
        {
            var recordTypes = assembly.GetTypes()
                .Where(t => t.IsClass && t.IsRecord() && t.Name.EndsWith("UseCases"));

            foreach (var recordType in recordTypes)
            {
                services.AddScoped(recordType);

                var ctor = recordType.GetConstructors().FirstOrDefault();
                if (ctor == null) continue;

                foreach (var parameter in ctor.GetParameters())
                {
                    services.AddScoped(parameter.ParameterType);
                }
            }
        }

        private static void RegisterUseCaseHandlers(IServiceCollection services, Assembly assembly)
        {
            var useCaseTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace?.Contains("UseCases") == true && t.Name != nameof(UseCasesDependencyInjection));

            foreach (var type in useCaseTypes)
            {
                // No-op edit to trigger apply_patch grouping (keeps behavior unchanged)
                services.AddScoped(type);
            }
        }

        private static bool IsRecord(this Type type)
        {
            // Detecta los records
            return type.GetMethod("<Clone>$") != null || type.GetMethod("PrintMembers", BindingFlags.NonPublic | BindingFlags.Instance) != null;
        }
    }
}
