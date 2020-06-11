using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Communication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace StatelessBankingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BankingController : ControllerBase
    {
        private readonly ILogger<BankingController> _logger;

        public BankingController(ILogger<BankingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("deposit")]
        public async Task<Decimal> Deposit(
            [FromQuery] string id,
            [FromQuery] string accountType,
            [FromQuery] Decimal amount)
        {
            var proxy = ServiceProxy.Create<IStatefulService>(
                new Uri("fabric:/FirstSFProj/StatefulBankingService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(accountType.ToUpperInvariant()));

            return await proxy.Deposit(id, amount);
        }

        [HttpGet]
        [Route("insights")]
        public async Task<string> GetInsightsStateless()
        {
            var proxy = ServiceProxy.Create<IStatelessService>(new Uri("fabric:/FirstSFProj/FirstStatelessService"));
            return await proxy.GetCustomerInsights();
        }

        [HttpGet]
        [Route("accounts")]
        public async Task<IEnumerable<Account>> GetAccounts(
            [FromQuery] string name,
            [FromQuery] string accountType)
        {
            var proxy = ServiceProxy.Create<IStatefulService>(
                new Uri("fabric:/FirstSFProj/StatefulBankingService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(accountType.ToUpperInvariant()));

            return await proxy.GetAccounts(name);
        }

        [HttpPost]
        [Route("Register")]
        public async Task Register([FromBody] Account account)
        {
            var proxy = ServiceProxy.Create<IStatefulService>(
                new Uri("fabric:/FirstSFProj/StatefulBankingService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(account.AccountType.ToUpperInvariant()));

            await proxy.Register(account);
        }

        [HttpGet]
        [Route("transfer")]
        public async Task Transfer(
            [FromQuery] string source,
            [FromQuery] string destination,
            [FromQuery] string accountType,
            [FromQuery] Decimal amount)
        {
            var proxy = ServiceProxy.Create<IStatefulService>(
                new Uri("fabric:/FirstSFProj/StatefulBankingService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(accountType.ToUpperInvariant()));

            await proxy.Transfer(source, destination, amount);
        }

    }
}
