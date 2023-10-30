using System;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace WEB_API.Common.Dependency_injection
{
    public class SaboxPaypal
    {
        private readonly IConfiguration _configuration;

    public SaboxPaypal(IConfiguration configuration)
    {
        _configuration = configuration;
    }

        public PayPalHttpClient client()
        {
            
            PayPalEnvironment _enviroment = new SandboxEnvironment(_configuration.GetSection("ConnectionStrings:PAYPAL-CLIENT-ID").Value, _configuration.GetSection("ConnectionStrings:PAYPAL-CLIENT-SECRET").Value);
            // create a lient for the enviroment 
            PayPalHttpClient client = new PayPalHttpClient(_enviroment);
            return client;
        }

    }
}
