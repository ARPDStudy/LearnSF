using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace StatefulBankingService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class StatefulBankingService : StatefulService, IStatefulService
    {
        const string AccountsByUser = "ACCOUNTSBYUSER";
        const string AccountsById = "ACCOUNTSBYID";

        public StatefulBankingService(StatefulServiceContext context)
            : base(context)
        {
            
        }

        public async Task<decimal> Deposit(string accountId, decimal amount)
        {
            var allAccounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsById);
            Decimal result = 0;
            using (var tx = this.StateManager.CreateTransaction())
            {
                var accounts = await allAccounts.TryGetValueAsync(tx, accountId);
                accounts.Value.Balance += amount;
                result = accounts.Value.Balance;

                await tx.CommitAsync();
            }

            return result;
        }

        public async Task<Account> GetAccount(string accountId)
        {
            var allAccounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsById);
            Account result = null;
            using (var tx = this.StateManager.CreateTransaction())
            {
                var accounts = await allAccounts.TryGetValueAsync(tx, accountId);
                if (accounts.HasValue)
                {
                    result = accounts.Value;
                }
                await tx.CommitAsync();
            }

            return result;
        }

        public async Task<IEnumerable<Account>> GetAccounts(string name)
        {
            var allAccounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, IEnumerable<Account>>>(AccountsByUser);
            IEnumerable<Account> result = null;
            using (var tx = this.StateManager.CreateTransaction())
            {
                var accounts = await allAccounts.TryGetValueAsync(tx, name);
                result = accounts.HasValue ? accounts.Value : new List<Account>();
                await tx.CommitAsync();
            }

            return result;
        }

        public async Task Register(Account account)
        {
            var accountsByUser = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, IEnumerable<Account>>>(AccountsByUser);
            var accountsById = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsById);

            using (var tx = this.StateManager.CreateTransaction())
            {
                await accountsById.AddOrUpdateAsync(tx, account.Id, account, (k, v) => v);
                await accountsByUser.AddOrUpdateAsync(
                    tx,
                    account.Name,
                    new List<Account> { account },
                    (k, v) =>
                    {
                        var existingAccount = v.FirstOrDefault(a => a.Id == account.Id);
                        if (existingAccount != null)
                        {
                            existingAccount.Update(account);
                        }
                        else
                        {
                            v.Append(account);
                        }

                        return v;
                    });

                await tx.CommitAsync();
            }
        }

        public async Task Transfer(string sourceAccountId, string destinationAccountId, decimal amount)
        {
            var allAccounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsById);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var source = await allAccounts.TryGetValueAsync(tx, sourceAccountId);
                var destination = await allAccounts.TryGetValueAsync(tx, destinationAccountId);
                source.Value.Balance -= amount;
                destination.Value.Balance += amount;

                await tx.CommitAsync();
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
