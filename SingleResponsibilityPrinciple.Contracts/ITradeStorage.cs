using System.Collections.Generic;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple.Contracts
{
    public interface ITradeStorage
    {
        Task PersistAsync(IEnumerable<TradeRecord> trades);  // Asynchronous version of Persist
    }
}