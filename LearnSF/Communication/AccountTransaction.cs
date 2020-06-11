using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Communication
{
    public class AccountTransaction
    {
        [DataMember]
        public string TransactionId { get; set; }

        [DataMember]
        public string Descrition { get; set; }

        [DataMember]
        public DateTimeOffset ModifiedOn { get; set; }

        [DataMember]
        public Decimal Amount { get; set; }
    }
}
