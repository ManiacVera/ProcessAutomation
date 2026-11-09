using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using ProcessAutomation.Api.Adapter.Service;
using ProcessAutomationMAUI.Base;
using ProcessAutomationMAUI.DataAccess;
using ProcessAutomationMAUI.Pages.Common;
using ProcessAutomationMAUI.Pages.Process;
using ProcessAutomationMAUI.Pages.Security;
using ProcessAutomationMAUI.ViewModel;
using System.Globalization;
using ApiConstants = ProcessAutomation.Api.Adapter.DTO.Constant.ProcessApiConstants;

namespace ProcessAutomationMAUI
{
    public static class MauiProgram
    {
        private static void CopyAllFormAssets()
        {
            List<string> fileList = new List<string>
            {
                { @"ConfigAdapter.json"},
                { @"Form/InputFields.html"},
                { @"Form/OutputValueOption.html"},
                { @"Scripts/AngularJS/angular/angular-translate-loader-partial.min.js"},
                { @"Scripts/AngularJS/angular/angular-translate.min.js"},
                { @"Scripts/AngularJS/angular/angular.min.js"},
                { @"Scripts/AngularJS/angular/angularjs-dropdown-multiselect.min.js"},
                { @"Scripts/AngularJS/angular/datepicker.js"},
                { @"Scripts/AngularJS/angular/generic-scripts.js"},
                { @"Scripts/AngularJS/angular/jquery-ui/external/jquery/jquery.js"},
                { @"Scripts/AngularJS/angular/jquery-ui/images/ui-icons_444444_256x240.png"},
                { @"Scripts/AngularJS/angular/jquery-ui/images/ui-icons_555555_256x240.png"},
                { @"Scripts/AngularJS/angular/jquery-ui/images/ui-icons_777620_256x240.png"},
                { @"Scripts/AngularJS/angular/jquery-ui/images/ui-icons_777777_256x240.png"},
                { @"Scripts/AngularJS/angular/jquery-ui/images/ui-icons_cc0000_256x240.png"},
                { @"Scripts/AngularJS/angular/jquery-ui/images/ui-icons_ffffff_256x240.png"},
                { @"Scripts/AngularJS/angular/jquery-ui/jquery-ui.css"},
                { @"Scripts/AngularJS/angular/jquery-ui/jquery-ui.js"},
                { @"Scripts/AngularJS/angular/jquery-ui/jquery-ui.min.css"},
                { @"Scripts/AngularJS/angular/jquery-ui/jquery-ui.min.js"},
                { @"Scripts/AngularJS/angular/jquery-ui/jquery-ui.structure.css"},
                { @"Scripts/AngularJS/angular/jquery-ui/jquery-ui.structure.min.css"},
                { @"Scripts/AngularJS/angular/jquery-ui/jquery-ui.theme.css"},
                { @"Scripts/AngularJS/angular/jquery-ui/jquery-ui.theme.min.css"},
                { @"Scripts/AngularJS/angular/jquery.min.js"},
                { @"Scripts/AngularJS/angular/mentio.js"},
                { @"Scripts/AngularJS/css/bootstrap-grid.css"},
                { @"Scripts/AngularJS/css/bootstrap-grid.css.map"},
                { @"Scripts/AngularJS/css/bootstrap-grid.min.css"},
                { @"Scripts/AngularJS/css/bootstrap-grid.min.css.map"},
                { @"Scripts/AngularJS/css/bootstrap-icons.css"},
                { @"Scripts/AngularJS/css/bootstrap-icons.json"},
                { @"Scripts/AngularJS/css/bootstrap-icons.min.css"},
                { @"Scripts/AngularJS/css/bootstrap-icons.scss"},
                { @"Scripts/AngularJS/css/bootstrap-reboot.css"},
                { @"Scripts/AngularJS/css/bootstrap-reboot.css.map"},
                { @"Scripts/AngularJS/css/bootstrap-reboot.min.css"},
                { @"Scripts/AngularJS/css/bootstrap-reboot.min.css.map"},
                { @"Scripts/AngularJS/css/bootstrap.css"},
                { @"Scripts/AngularJS/css/bootstrap.css.map"},
                { @"Scripts/AngularJS/css/bootstrap.min.css"},
                { @"Scripts/AngularJS/css/bootstrap.min.css.map"},
                { @"Scripts/AngularJS/css/fonts/bootstrap-icons.woff"},
                { @"Scripts/AngularJS/css/fonts/bootstrap-icons.woff2"},
                { @"Scripts/AngularJS/css/main.css"},
                { @"Scripts/AngularJS/css/spinner.css"},
                { @"Scripts/AngularJS/js/bootstrap.bundle.js"},
                { @"Scripts/AngularJS/js/bootstrap.bundle.js.map"},
                { @"Scripts/AngularJS/js/bootstrap.bundle.min.js"},
                { @"Scripts/AngularJS/js/bootstrap.bundle.min.js.map"},
                { @"Scripts/AngularJS/js/bootstrap.js"},
                { @"Scripts/AngularJS/js/bootstrap.js.map"},
                { @"Scripts/AngularJS/js/bootstrap.min.js"},
                { @"Scripts/AngularJS/js/bootstrap.min.js.map"},
                { @"Scripts/jquery/jquery-1.12.5.min.js"},
                { @"Scripts/jquery/jquery-3.5.1.min.js"},
                { @"Scripts/jquery/jquery-ui.min.js"},
                { @"Scripts/tagit/tag-it.js"}
            };

            Task copyFileTask = null;
            foreach (string file in fileList)
            {
                copyFileTask = Task.Run(async () =>
                {
                    await Utilities.CopyFileToAppDataDirectory(file);
                });

                copyFileTask.Wait();
            }
        }


        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa-solid-900.ttf", "FaSolid");
                });

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
#if ANDROID
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
            });
            
            
            builder.Services.AddDbContext<StoreDbContext>();

            Task commonTask = Task.Run(async () =>
            {
                StoreDbContext dbContext = new StoreDbContext();
                await dbContext.ExecuteScriptDatabase();
                dbContext.Dispose();

                await Utilities.CopyFileToAppDataDirectory(ApiConstants.CONFIG_FILE);
            });

            CopyAllFormAssets();

            commonTask.Wait();

            List<CultureInfo> cultureList = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("es-MX") };

            LanguageManager.SetLanguage();
            //Preferences.Set("language", defaultCulture.Name);

            //builder.Services.Configure<RequestLocalizationOptions>(options =>
            //{
            //    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es-MX");
            //    options.SupportedUICultures = cultureList;
            //});

            builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
            builder.Services.AddScoped<ProcessApiAdapter>();
            builder.Services.AddTransient<MainVM>();
            builder.Services.AddTransient<MainPage>();

            builder.Services.AddTransient<AccountVM>();
            builder.Services.AddTransient<ComponentVM>();
            //builder.Services.AddTransient<InputValueVM>();
            builder.Services.AddTransient<LoginVM>();            
            //builder.Services.AddTransient<OutputValueVM>();
            builder.Services.AddTransient<ProcessVM>();
            builder.Services.AddTransient<SplashVM>();
            builder.Services.AddTransient<StatementVM>();
            builder.Services.AddTransient<StepVM>();
            builder.Services.AddTransient<TaskVM>();

            builder.Services.AddTransient<AccountPage>();
            builder.Services.AddTransient<ComponentPage>();
            //builder.Services.AddTransient<InputValuePage>();
            builder.Services.AddTransient<LoginPage>();
            //builder.Services.AddTransient<OutputValuePage>();
            builder.Services.AddTransient<ProcessPage>();
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<StatementPage>();
            builder.Services.AddTransient<StepPage>();
            builder.Services.AddTransient<TaskPage>();

            //Routing.RegisterRoute(nameof(AccountPage), typeof(AccountPage));
            //Routing.RegisterRoute(nameof(InputValuePage), typeof(InputValuePage));
            Routing.RegisterRoute(nameof(ComponentPage), typeof(ComponentPage));
            //Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            //Routing.RegisterRoute(nameof(OutputValuePage), typeof(OutputValuePage));
            //Routing.RegisterRoute(nameof(ProcessPage), typeof(ProcessPage));
            Routing.RegisterRoute(nameof(StatementPage), typeof(StatementPage));
            Routing.RegisterRoute(nameof(StepPage), typeof(StepPage));
            Routing.RegisterRoute(nameof(TaskPage), typeof(TaskPage));
            //Routing.RegisterRoute(nameof(SplashPage), typeof(SplashPage));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
