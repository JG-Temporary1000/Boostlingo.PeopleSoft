using Polly.Extensions.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Boostlingo.PeopleSoft.Business.Helpers
{
    public class HttpCircuitBreakerPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
