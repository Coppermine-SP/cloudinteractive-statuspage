using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using Ng.Services;
using cloudinteractive_statuspage.Services;
using cloudinteractive_statuspage.Services.Watchdog;

namespace cloudinteractive_statuspage
{
    public class Program
    {
        private static ILogger logger;
        private static bool isDevelopement = true;
        public static void Main(string[] args)
        {
            Console.WriteLine("cloudinteractive_statuspage - Server\nCopyright (C) 2017-2023 CloudInteractive Inc.\n\n");
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddUserAgentService();
            builder.Services.AddSingleton<Configuration.ConfigService>(x =>
                new Configuration.ConfigService(x));
            builder.Services.AddSingleton<NotifyService>(x =>
                new NotifyService(x));
            builder.Services.AddSingleton<ObserverPoolService>( x=>
                new ObserverPoolService(x));


            var app = builder.Build();
            var config = app.Services.GetService<Configuration.ConfigService>();
            var notifyService = app.Services.GetService<NotifyService>();
            var observerPoolService = app.Services.GetService<ObserverPoolService>();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                isDevelopement = false;
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Load configurations.
            try
            {
                config?.Init();
            }
            catch (Exception e)
            {
                ExitWithError(e);
            }

            // observerPoolService Init.
            try
            {
                observerPoolService?.Init();
            }
            catch (Exception e)
            {
                ExitWithError(e);
            }

            // Load notifications from file.
            notifyService?.LoadFromFile();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        public static void ExitWithError(Exception e)
        {
            logger.LogCritical("Program has encountered a problem and need to close. We are sorry for the Inconvenience. ");
            logger.LogCritical("CriticalException : " + e);

            if (isDevelopement)
            {
                Console.WriteLine("\nPress any key to exit..");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}