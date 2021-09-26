using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tasks4U.Data;

namespace Tasks4U.Services
{
    public class MigratorService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MigratorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                ILogger<MigratorService> logger = scope.ServiceProvider.GetService<ILogger<MigratorService>>();
                logger.LogInformation("Migrating database.");
                Tasks4UDbContext dbContext = scope.ServiceProvider.GetRequiredService<Tasks4UDbContext>();
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("DataBase Successfully migrated");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
