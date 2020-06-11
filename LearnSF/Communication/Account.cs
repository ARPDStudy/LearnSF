using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Communication
{
    public class Account
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string AccountType { get; set; }

        [DataMember]
        public Decimal Balance { get; set; }

        [DataMember]
        public IEnumerable<AccountTransaction> History { get; set; } = new List<AccountTransaction>();

        public void Update(Account account)
        {
            this.Id = account.Id;
            this.Name = account.Name;
            this.AccountType = account.AccountType;
            this.Balance = account.Balance;
            var history = account.History;
        }
    }
}
