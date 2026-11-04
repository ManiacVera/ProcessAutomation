using Microsoft.Extensions.DependencyInjection;
using ProcessAutomation.Framework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProcessAutomation.Framework.Manager
{
    public sealed class ServiceLoader
    {
        public static bool ExistsService<TService>(IServiceCollection serviceCollection) where TService : class
        {
            return (serviceCollection != null && serviceCollection.Any(f => f.ServiceType == typeof(TService)));
        }

        public static void AddScoped<TService, TImplementation>(IServiceCollection serviceCollection) where TService : class where TImplementation : class, TService
        {
            if (!ExistsService<TService>(serviceCollection))
            {
                serviceCollection.AddScoped<TService, TImplementation>();

                if (BaseRepository.GetInstance() != null)
                {
                    BaseRepository.GetInstance().AddLibrary<TService, TImplementation>();
                }
            }
        }

        public static void AddTransient<TService, TImplementation>(IServiceCollection serviceCollection) where TService : class where TImplementation : class, TService
        {
            if (!ExistsService<TService>(serviceCollection))
            {
                serviceCollection.AddTransient<TService, TImplementation>();

                if (BaseRepository.GetInstance() != null)
                {
                    BaseRepository.GetInstance().AddLibrary<TService, TImplementation>();
                }
            }
        }

        public static void AddSingleton<TService, TImplementation>(IServiceCollection serviceCollection) where TService : class where TImplementation : class, TService
        {
            if (!ExistsService<TService>(serviceCollection))
            {
                serviceCollection.AddSingleton<TService, TImplementation>();

                if (BaseRepository.GetInstance() != null)
                {
                    BaseRepository.GetInstance().AddLibrary<TService, TImplementation>();
                }
            }
        }

        public static void AddSingleton<TService>(TService implementationFactory, IServiceCollection serviceCollection) where TService : class
        {
            if (!ExistsService<TService>(serviceCollection))
            {
                serviceCollection.AddSingleton(implementationFactory);

                if (BaseRepository.GetInstance() != null)
                {
                    BaseRepository.GetInstance().AddLibrary<TService, TService>();
                }
            }
        }

        public static void AddScopedFromReflection(IServiceCollection serviceCollection, Assembly assembly, string namespaceContract, string namespaceService)
        {
            MethodInfo addScopedMethod = typeof(ServiceLoader).GetMethod("AddScoped");
            List<Type> namespaceList = assembly.GetTypes().Where(f => f.Namespace == namespaceContract).ToList();
            List<Type> classList = assembly.GetTypes().Where(f => f.Namespace == namespaceService).ToList();
            Type namespaceType = null;

            if (addScopedMethod != null)
            {
                MethodInfo genericMethod = null;
                foreach (Type classType in classList)
                {
                    namespaceType = namespaceList.FirstOrDefault(f => f.Name.Contains(classType.Name));
                    if (namespaceType != null)
                    {
                        genericMethod = addScopedMethod.MakeGenericMethod(namespaceType, classType);
                        genericMethod.Invoke(null, new object[] { serviceCollection });
                    }
                }
            }
        }
    }
}
