using System;
using System.Reflection;
using System.Threading.Tasks;
using SingleResponsibilityPrinciple.AdoNet;
using SingleResponsibilityPrinciple.Contracts;

namespace SingleResponsibilityPrinciple
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ILogger logger = new ConsoleLogger();
            // Open up the local text file as a stream
            String fileName = "SingleResponsibilityPrinciple.trades.txt";
            Stream tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
            if (tradeStream == null)
            {
                logger.LogWarning("Trade file could not be opened at " + fileName);
                Environment.Exit(1); // Exits the application with a non-zero exit code indicating an error
            }

            // URL to read trade file from
            string tradeURL = "http://raw.githubusercontent.com/tgibbons-css/CIS3285_Unit9_F24/refs/heads/master/SingleResponsibilityPrinciple/trades.txt";
            string restfulURL = "http://unit9trader.azurewebsites.net/api/TradeData";

            ITradeValidator tradeValidator = new SimpleTradeValidator(logger);

            // These are trade providers that read from different sources
            ITradeDataProvider urlProvider = new URLTradeDataProvider(tradeURL, logger);
            ITradeDataProvider adjustProvider = new AdjustTradeDataProvider(urlProvider);
            ITradeDataProvider restfulProvider = new RestfulTradeDataProvider(restfulURL, logger);
            ITradeDataProvider asyncProvider = new URLAsyncProvider(adjustProvider, logger);

            ITradeMapper tradeMapper = new SimpleTradeMapper();
            ITradeParser tradeParser = new SimpleTradeParser(tradeValidator, tradeMapper);
            ITradeStorage tradeStorage = new AdoNetTradeStorage(logger);

            TradeProcessor tradeProcessor = new TradeProcessor(asyncProvider, tradeParser, tradeStorage);

            await tradeProcessor.ProcessTradesAsync();  

            // Optionally, await Console.ReadKey(); if needed for debugging purposes.
        }
    }
}
