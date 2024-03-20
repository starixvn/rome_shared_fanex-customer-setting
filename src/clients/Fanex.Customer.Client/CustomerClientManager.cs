using System;
using Fanex.Customer.Client.Configurations;

namespace Fanex.Customer.Client
{
    public class CustomerClientManager
    {
        private CustomerClientConfiguration clientConfigs;
        private static readonly CustomerClientManager CurrentInstance = new CustomerClientManager();

        public static CustomerClientManager UseConfig(CustomerClientConfiguration clientConfigs)
        {
            CurrentInstance.clientConfigs = clientConfigs ?? throw new ArgumentNullException(nameof(clientConfigs));

            return CurrentInstance;
        }

        public static CustomerClientConfiguration ClientConfigs
            => CurrentInstance.clientConfigs ?? throw new InvalidOperationException(nameof(CurrentInstance.clientConfigs));
    }
}
