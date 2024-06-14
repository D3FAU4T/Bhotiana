using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace Bhotiana
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var ServiceProvider = Startup.ConfigureServices();
            var HttpClient = ServiceProvider.GetService<HttpClient>();
            var SpeechSynthesizer = ServiceProvider.GetService<SpeechSynthesizer>();
            Application.Run(new App(SpeechSynthesizer, HttpClient));
        }
    }

    public class Startup
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<HttpClient>();
            services.AddTransient<SpeechSynthesizer>();

            return services.BuildServiceProvider();
        }
    }
}
