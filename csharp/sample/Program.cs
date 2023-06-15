namespace Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CSE.ConfigMgmt;
using CSE.ConfigMgmt.Getters;
using Azure.Identity;
using Azure.Data.AppConfiguration;
using CSE.ConfigMgmt.Transforms;

/// <summary>
/// The application.
/// </summary>
internal class Program
{
    /// <summary>/
    /// The entry point of the application.
    /// </summary>
    /// <param name="args">The arguments.</param>
    internal static void Main(string[] args)
    {
        // create the host
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = false;
                    options.SingleLine = true;
                });
                logging.SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                // add hosted services
                services.AddHostedService<AppLifecycle>();

                // add config
                services.AddConfigMgmt<ConfigGetter, EnvGetter>();
                services.AddSingleton<IGetter>(provider =>
                {
                    return new AzureAppConfigGetter(
                        "https://pelasne-config.azconfig.io",
                        new SettingSelector { LabelFilter = "dev", KeyFilter = "/myapp/*" },
                        provider.GetService<DefaultAzureCredential>(),
                        provider.GetService<ILogger<AzureAppConfigGetter>>());
                });
                services.AddSingleton<DefaultAzureCredential>(provider =>
                {
                    return new DefaultAzureCredential(new DefaultAzureCredentialOptions
                    {
                        ExcludeEnvironmentCredential = true,
                        ExcludeManagedIdentityCredential = true,
                        ExcludeSharedTokenCacheCredential = true,
                        ExcludeVisualStudioCredential = true,
                        ExcludeVisualStudioCodeCredential = true,
                        ExcludeAzureCliCredential = false, // support only AZ CLI
                        ExcludeInteractiveBrowserCredential = true,
                    });
                }); // this is optional, but makes it faster by specifying how it can authenticate
                services.AddSingleton<IAzureKeyVaultTransform, AzureKeyVaultTransform>();

                // add application services
                services.AddSingleton<IConfig, Config>();
            });

        host.Build().Run();
    }
}
