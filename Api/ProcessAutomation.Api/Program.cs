using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessAutomation.Business;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Framework.Base;
using ProcessAutomation.Framework.Manager;
using ProcessAutomation.Framework.Shared;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json.Serialization;
using ProcessAutomation.Framework.DTO.Enum;

public class Program
{

    private static string assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    public static void Main(string[] args)
    {
        StartupManager startupManager = new StartupManager(assemblyPath);
        AssemblyLoadContext.Default.Resolving += startupManager.DefaultAssemblyContextResolve;
        AssemblyLoadContext.Default.ResolvingUnmanagedDll += startupManager.DefaultAssemblyContextResolveUnmanaged;

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
        {
            #if ENABLE_TOKEN_VALIDATION
            options.Filters.Add<ValidateTokenAttribute>();
            #endif
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        });

        builder.Services.AddMemoryCache();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        EncryptionShared.InitializeEncryption(EncryptionAlgorithmTypeEnum.AESGCM, EncryptionConstants.ENCRYPTION_KEY);
        BaseRepository.Create();
        BusinessLoader.Load(builder.Services);
        //BaseRepository.GetInstance().SetServiceProviderReadOnly(builder.Services.BuildServiceProvider());        

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();

        AssemblyLoadContext.Default.Resolving -= startupManager.DefaultAssemblyContextResolve;
        AssemblyLoadContext.Default.ResolvingUnmanagedDll -= startupManager.DefaultAssemblyContextResolveUnmanaged;
    }
}