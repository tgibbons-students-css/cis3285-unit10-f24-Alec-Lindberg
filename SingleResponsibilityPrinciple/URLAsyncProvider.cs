using SingleResponsibilityPrinciple.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private async Task<List<string>> GetTradeAsync()
        {
            _logger.LogInfo("Fetching trade data asynchronously.");
            List<string> tradesString = null;

            // Call the base provider's GetTradeData (in this case, might be a URL-based provider)
            var data = _baseProvider.GetTradeData();

            // Here, if _baseProvider is fetching from a URL, you could handle parsing or adjust here
            foreach (var item in data)
            {
                // Assuming item might be a JSON string or other structure you might adjust or handle
                tradesString.Add(item.Replace("GBP", "EUR"));
            }

            _logger.LogInfo("Received trade strings of length = " + tradesString.Count);
            return tradesString;
        }

        public IEnumerable<string> GetTradeData()
        {
            Task<List<string>> task = Task.Run(() => GetTradeAsync());
            task.Wait();

            List<string> tradeList = task.Result;
            return tradeList;
        }

    }
}
