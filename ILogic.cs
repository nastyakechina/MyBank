using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter;

    public interface ILogic
    {
        Task AddNewCoinAsync(Coin coin, CancellationToken cancellationToken);
        Task DepositWalletAsync(string coinName, decimal amount, CancellationToken cancellationToken);
        Task<decimal> ConversionCoinAsync(string fromCurrency, string toCurrency, decimal amount, CancellationToken cancellationToken);
        Task<Dictionary<string, decimal>> GetBalanceAsync(CancellationToken cancellationToken);
        Task<List<Transaction>> GetHistoryAsync(CancellationToken cancellationToken);
    }