using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ePortal.Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ePortal.Application.Contracts;

namespace ePortal.Infrastructure.BackgroundServices
{
    public class AutoRejectItemsTask : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        public AutoRejectItemsTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var lostAndFoundService = scope.ServiceProvider.GetRequiredService<ILostAndFoundService>();
                        var lostAndFoundRepository = scope.ServiceProvider.GetRequiredService<LostAndFoundRepository>();

                        var itemsToReject = lostAndFoundRepository.GetPendingItemsOlderThanWorkingDays(7);

                        foreach (var item in itemsToReject)
                        {
                            var result = lostAndFoundService.RejectItem(item.Id);
                            if (result)
                                Console.WriteLine($"Item with ID {item.Id} has been auto-rejected.");
                            else
                                Console.WriteLine($"Failed to auto-reject item with ID {item.Id}.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in AutoRejectItemsTask: {ex.Message}");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
