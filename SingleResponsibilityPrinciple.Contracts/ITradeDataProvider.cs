using System.Collections.Generic;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple.Contracts
{
    public interface ITradeDataProvider
    {
        IAsyncEnumerable<string> GetTradeDataAsync();
    }
}