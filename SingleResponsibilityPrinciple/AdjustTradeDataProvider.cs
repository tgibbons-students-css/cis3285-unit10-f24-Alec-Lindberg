using SingleResponsibilityPrinciple.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple
{
    public class AdjustTradeDataProvider : ITradeDataProvider
    {
        private readonly ITradeDataProvider _origProvider;

        public AdjustTradeDataProvider(ITradeDataProvider origProvider)
        {
            _origProvider = origProvider;
        }

        public async IAsyncEnumerable<string> GetTradeDataAsync()
        {
            await foreach (string line in _origProvider.GetTradeDataAsync())
            {
                // Replace "GBP" with "EUR" in each line and yield the modified line
                string newLine = line.Replace("GBP", "EUR");
                yield return newLine;
            }
        }
    }
}
