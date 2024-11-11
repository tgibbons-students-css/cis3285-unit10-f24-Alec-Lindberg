using SingleResponsibilityPrinciple.Contracts;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple
{
    public class URLAsyncProvider : ITradeDataProvider
    {
        private readonly ITradeDataProvider _baseProvider;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ILogger _logger;

        public URLAsyncProvider(ITradeDataProvider baseProvider, ILogger logger)
        {
            _baseProvider = baseProvider;
            _logger = logger;
        }

        public async IAsyncEnumerable<string> GetTradeDataAsync()
        {
            _logger.LogInfo("Fetching trade data asynchronously.");

            // Fetch data asynchronously from the base provider.
            await foreach (var item in _baseProvider.GetTradeDataAsync())
            {
                // Replace "GBP" with "EUR" in each item before yielding.
                yield return item.Replace("GBP", "EUR");
            }

            _logger.LogInfo("Trade data fetching complete.");
        }
    }
}
