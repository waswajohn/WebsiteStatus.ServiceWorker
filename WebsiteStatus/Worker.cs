using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteStatus
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        //LOAD WEBPAGE USING HTTP CLIENT OPEN HTTP CLIENT ONCE TO AVOID CLOGGING UP YOUR COMPUTER
        private HttpClient client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            client.Dispose();
            _logger.LogInformation("The Service has been stopped.....");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await client.GetAsync("https://morvey.com ");
                
                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation("The Website is up. Status Code {StatusCode}", result.StatusCode);
                }

                else
                {
                    _logger.LogError("The Website is down. Status Code {StatusCode}", result.StatusCode);
                }

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken); //5 SECONDS
            }
        }
    }
}
