namespace Client;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// This hosted service allows for startup and shutdown activities related to the application itself.
/// </summary>
internal class AppLifecycle : IHostedService
{
    private readonly IConfig config;
    private readonly ILogger<AppLifecycle> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppLifecycle"/> class.
    /// </summary>
    /// <param name="config">The configuration for this application.</param>
    /// <param name="logger">The logger.</param>
    public AppLifecycle(IConfig config, ILogger<AppLifecycle> logger)
    {
        this.config = config;
        this.logger = logger;
    }

    /// <summary>
    /// This method should contain all startup activities for the application.
    /// </summary>
    /// <param name="cancellationToken">A token that can be cancelled to abort startup.</param>
    /// <returns>A Task that is complete when the method is done.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var validationResult = this.config.Validate();
        if (validationResult.IsFailed)
        {
            foreach (var error in validationResult.Errors)
            {
                this.logger.LogError(error.Message);
            }

            return Task.FromException(new System.Exception("Configuration validation failed."));
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// This method should contain all shutdown activities for the application.
    /// </summary>
    /// <param name="cancellationToken">A token that can be cancelled to abort startup.</param>
    /// <returns>A Task that is complete when the method is done.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}