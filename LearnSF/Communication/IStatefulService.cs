using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace Communication
{
    public interface IStatefulService : IService
    {
        Task<Account> GetAccount(string name);

        Task<Decimal> Deposit(string accountId, Decimal amount);

        Task<Decimal> WithDraw(string accountId, Decimal amount);

        Task Transfer(string sourceAccountId, string destinationAccountId, Decimal amount);
    }
}
