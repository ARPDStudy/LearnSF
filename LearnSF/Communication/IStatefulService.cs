using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication
{
    public interface IStatefulService : IService
    {
        Task<IEnumerable<Account>> GetAccounts(string name);

        Task<Decimal> Deposit(string accountId, Decimal amount);

        Task Register(Account account);

        Task Transfer(string sourceAccountId, string destinationAccountId, Decimal amount);
    }
}
