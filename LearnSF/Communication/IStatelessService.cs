﻿using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public interface IStatelessService : IService
    {
        Task<string> GetCustomerInsights();
    }
}
