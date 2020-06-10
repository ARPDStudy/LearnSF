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
        public IEnumerable<Transaction> History { get; set; } = new List<Transaction>();
    }
}
