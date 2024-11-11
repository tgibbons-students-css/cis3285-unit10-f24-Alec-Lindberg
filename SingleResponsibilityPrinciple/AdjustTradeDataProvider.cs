using SingleResponsibilityPrinciple.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple
{
    public class AdjustTradeDataProvider : ITradeDataProvider
    {
        ITradeDataProvider origProvider;

       public AdjustTradeDataProvider(ITradeDataProvider origProvider)
        {
            this.origProvider = origProvider;
        }

        public IEnumerable<string> GetTradeData()
        {
         IEnumerable<string> lines = origProvider.GetTradeData();

            List<string> result = new List<string>();

            foreach (string line in lines)
            {
                String newLine = line.Replace("GBP", "EUR");
                result.Add(line);
            }
            return lines; 
        }
    }
}
