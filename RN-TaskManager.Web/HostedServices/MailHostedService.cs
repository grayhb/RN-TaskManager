using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RN_TaskManager.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RN_TaskManager.Web.HostedServices
{
    public class MailHostedService : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        public MailHostedService (IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void DoWork(object state)
        {
            using var scope = _scopeFactory.CreateScope();

            var mailRepository = scope.ServiceProvider.GetRequiredService<IMailRepository>();

            mailRepository.CreateMailsAsync().Wait();

            mailRepository.SendMails().Wait();
        }
    }
}
