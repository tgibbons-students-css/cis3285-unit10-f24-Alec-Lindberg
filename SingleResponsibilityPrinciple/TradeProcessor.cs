
using SingleResponsibilityPrinciple.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple
{
    public class TradeProcessor
    {
        public TradeProcessor(ITradeDataProvider tradeDataProvider, ITradeParser tradeParser, ITradeStorage tradeStorage)
        {
            this.tradeDataProvider = tradeDataProvider;
            this.tradeParser = tradeParser;
            this.tradeStorage = tradeStorage;
        }

        public async Task ProcessTradesAsync()
        {
            // Fetch trade data asynchronously
            var lines = new List<string>();
            await foreach (var line in tradeDataProvider.GetTradeDataAsync())
            {
                lines.Add(line);
            }

            // Parse trades from lines
            var trades = tradeParser.Parse(lines);

            // Persist trades to storage
            await tradeStorage.PersistAsync(trades); // Assuming PersistAsync is implemented
        }

        private readonly ITradeDataProvider tradeDataProvider;
        private readonly ITradeParser tradeParser;
        private readonly ITradeStorage tradeStorage;
    }
}
