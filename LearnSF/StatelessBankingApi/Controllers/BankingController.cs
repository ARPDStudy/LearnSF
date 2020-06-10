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
        [Route("insights")]
        public async Task<string> GetInsightsStateless()
        {
            var proxy = ServiceProxy.Create<IStatelessService>(new Uri("fabric:/FirstSFProj/FirstStatelessService"));
            return await proxy.GetCustomerInsights();
        }
    }
}
