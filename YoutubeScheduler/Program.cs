namespace YoutubeScheduler
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.UserSecrets;
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddUserSecrets<Form1>();
            var configuration = builder.Build();
            Application.Run(new Form1(configuration));
        }
    }
}
