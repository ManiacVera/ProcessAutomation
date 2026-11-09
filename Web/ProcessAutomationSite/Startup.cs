using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ProcessAutomation.Business;
using ProcessAutomation.Common.Model;
using ProcessAutomation.Framework.Base;
using ProcessAutomation.Framework.DTO.Enum;
using ProcessAutomationSite.DTO.Constant;
using ProcessAutomationSite.Service;
using Radzen;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ProcessAutomationSite
{
    public class Startup
    {
        private static string assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static dynamic configFile;
        public static string webApi;
        public static string routeApi;        
        //public delegate WebApiManager WebApiNotification();
        //public delegate WebApiManager WebApiChat();
        //public delegate WebApiManager WebApiUser();
        //public delegate WebApiManager WebApiPaymentType();
               

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler((object sender, ResolveEventArgs args) =>
            //{
            //    return StartupManager.AssemblyResolve(sender, args, Path.Combine(assemblyPath, Comm), assemblyPath);
            //});
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProcessAutomationSiteConstant.CONFIG_FOLDER, ProcessAutomationSiteConstant.CONFIG_FILE);
            using (StreamReader r = new StreamReader(configFilePath))
            {
                configFile = JsonConvert.DeserializeObject<dynamic>(r.ReadToEnd());
            }

            //configFile = DataSerialiazerManager.DeserializerToDictionary(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProcessAutomationSiteConstant.CONFIG_FOLDER, ProcessAutomationSiteConstant.CONFIG_FILE)));
            //webApi = configFile["WebApi"].ToString();
            //routeApi = configFile["RouteApi"].ToString();

            // LUIS VERA: HACER LA LOGICA PERO PARA CREAR LA CONEXIÓN DESDE RepositoryBase.cs
            if (configFile["DatabaseType"].ToString() == "LOCAL")
            {
                SiteContext.CreateStaticContext(@$"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SiteContext.database)}", DatabaseTypeEnum.SQLITE);
            }
            else
            {
                if (configFile.DatabaseServer.Port.ToString() == "0")
                {
                    SiteContext.CreateStaticContext(@$"Server={configFile.DatabaseServer.Server};Database={configFile.DatabaseServer.Database};User Id={configFile.DatabaseServer.UserId};Password={configFile.DatabaseServer.Password}", DatabaseTypeEnum.SQLSERVER);
                }
                else
                {
                    SiteContext.CreateStaticContext(@$"Server={configFile.DatabaseServer.Server},{configFile.DatabaseServer.Port.ToString()};Database={configFile.DatabaseServer.Database};User Id={configFile.DatabaseServer.UserId};Password={configFile.DatabaseServer.Password}", DatabaseTypeEnum.SQLSERVER);
                }
            }
            //services.ConfigureApplicationCookie(options => options.LoginPath = "/Login");
            services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddRazorPages();
            services.AddAuthorizationCore();
            services.AddLocalization(options => options.ResourcesPath = "i18n");
            List<CultureInfo> cultureList = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("es-MX") };

            CultureInfo defaultCulture = new CultureInfo("es-MX");
            CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
            CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
            Thread.CurrentThread.CurrentCulture = defaultCulture;
            Thread.CurrentThread.CurrentUICulture = defaultCulture;
            CultureInfo.CurrentCulture = defaultCulture;
            CultureInfo.CurrentUICulture = defaultCulture;

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es-MX");
                options.SupportedUICultures = cultureList;
            });

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //    {
            //        options.Cookie.Name = "PAUserCookie";
            //        options.LoginPath = "/Login";
            //        options.LogoutPath = "/Logoff";
            //        options.ExpireTimeSpan = TimeSpan.FromDays(100);
            //    });

            //services.AddAuthentication().AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];

            //    facebookOptions.Events = new OAuthEvents()
            //    {
            //        OnRemoteFailure = loginFailureHandler =>
            //        {
            //            var authProperties = facebookOptions.StateDataFormat.Unprotect(loginFailureHandler.Request.Query["state"]);
            //            loginFailureHandler.Response.Redirect("/Login");
            //            loginFailureHandler.HandleResponse();
            //            return Task.FromResult(0);
            //        }
            //    };
            //});

            services.AddHttpContextAccessor();
            services.AddScoped<AuthenticationStateService>();
            services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthenticationStateService>());
            //services.AddScoped<WebApiManager>(s => new WebApiManager(webApi, string.Format("{0}/{1}", Startup.routeApi, ProcessAutomationSiteConstant.PROCESSAUTOMATION_CONTROLLER)));            

            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<StateManagement>();

            BaseRepository.Create();
            BusinessLoader.Load(services);
            SiteContext.BuiderCommonDatabase();

            //BaseRepository.GetInstance().SetServiceProviderReadOnly(services.BuildServiceProvider());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shell/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
                RequestPath = "/Resources"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Scripts")),
                RequestPath = "/Scripts"
            });

            //app.UseRequestLocalization();

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
                RequestPath = "/Resources"
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Scripts")),
                RequestPath = "/Scripts"
            });
            
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "css")),
                RequestPath = "/css"
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "css")),
                RequestPath = "/css"
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/Shell/ShellPage");
            });
            
            //app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
        }
    }
}
