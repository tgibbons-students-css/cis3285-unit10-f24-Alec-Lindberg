using System;
using System.Reflection;

using SingleResponsibilityPrinciple.AdoNet;
using SingleResponsibilityPrinciple.Contracts;

namespace SingleResponsibilityPrinciple
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = new ConsoleLogger();
            // Open up the local textfile as a stream
            String fileName = "SingleResponsibilityPrinciple.trades.txt";
            Stream tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
            if (tradeStream == null)
            {
                logger.LogWarning("trade file could not be openned at " + fileName);
                Environment.Exit(1); // Exits the application with a non-zero exit code indicating an error
            }
            // data file to read from locally
            //Stream tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit9_Trader.trades.txt");
            
            // URL to read trade file from
            string tradeURL = "http://raw.githubusercontent.com/tgibbons-css/CIS3285_Unit9_F24/refs/heads/master/SingleResponsibilityPrinciple/trades.txt";
            //Two different URLs for Restful API
            //string restfulURL = "http://localhost:22359/api/TradeData";
            string restfulURL = "http://unit9trader.azurewebsites.net/api/TradeData";

            ITradeValidator tradeValidator = new SimpleTradeValidator(logger);

            //These are three different trade providers that read from different sources
           // ITradeDataProvider fileProvider = new StreamTradeDataProvider(tradeStream, logger);
            ITradeDataProvider urlProvider = new URLTradeDataProvider(tradeURL, logger);
            ITradeDataProvider adjustProvider = new AdjustTradeDataProvider(urlProvider);
            ITradeDataProvider restfulProvider = new RestfulTradeDataProvider(restfulURL, logger);
            ITradeDataProvider asyncProvider = new URLAsyncProvider(adjustProvider, logger);

            ITradeMapper tradeMapper = new SimpleTradeMapper();
            ITradeParser tradeParser = new SimpleTradeParser(tradeValidator, tradeMapper);
            ITradeStorage tradeStorage = new AdoNetTradeStorage(logger);

            TradeProcessor tradeProcessor = new TradeProcessor(adjustProvider, tradeParser, tradeStorage);
            //TradeProcessor tradeProcessor = new TradeProcessor(urlProvider, tradeParser, tradeStorage);

            tradeProcessor.ProcessTrades();

            //Console.ReadKey();


        }
    }
}
