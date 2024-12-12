using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Storage;

namespace Presenter
{
    public class Logic : ILogic
    {
        private readonly ICoinStorage _coinStorage;
        private readonly IWalletStorage _walletStorage;
        private readonly ITransactionStorage _transactionStorage;

        public Logic(ICoinStorage coinStorage, IWalletStorage walletStorage, ITransactionStorage transactionStorage)
        {
            _coinStorage = coinStorage;
            _walletStorage = walletStorage;
            _transactionStorage = transactionStorage;
            // _walletStorage.AddWalletAsync(new Wallet(new Guid(), "Кошелек"), new CancellationToken());
        }

        public async Task AddNewCoinAsync(Coin coin, CancellationToken cancellationToken)
        {
            if (coin.Course <= 0)
            {
                throw new WalletException("Курс не может быть неположительным числом.");
            }

            if (await _coinStorage.GetCoinAsync(coin.Name, cancellationToken) == null)
            {
                await _coinStorage.AddCoinAsync(coin, cancellationToken);
            }
            else
            {
                throw new WalletException($"Валюта {coin.Name} уже существует");
            }
        }

        public async Task DepositWalletAsync(string coinName, decimal amount, CancellationToken cancellationToken)
        {
            var wallets = await _walletStorage.GetAllWalletsAsync(cancellationToken);
            var wallet = wallets.FirstOrDefault(); // Предполагается, что есть только один кошелек

            if (wallet != null)
            {
                var coinAmount = await _walletStorage.GetCoinAmountAsync(wallet.id, coinName, cancellationToken);

                if (amount < 0)
                {
                    throw new WalletException("Сумма должна быть положительным числом.");
                }

                if (coinAmount != null)
                {
                    coinAmount.Amount += amount;
                    await _walletStorage.UpdateCoinAmountAsync(coinAmount, cancellationToken);
                }
                else
                {
                    var coin = await _coinStorage.GetCoinAsync(coinName, cancellationToken);
                    if (coin != null)
                    {
                        coinAmount = new CoinAmount(amount, coin.Name, wallet.id);
                        await _walletStorage.AddCoinAmountAsync(coinAmount, cancellationToken);
                    }
                    else
                    {
                        throw new WalletException($"Валюта {coinName} не найдена.");
                    }
                }

                await _transactionStorage.AddTransactionAsync(new Transaction(Guid.NewGuid(), "Пополнение", amount, DateTime.UtcNow), cancellationToken);
            }
            else
            {
                throw new WalletException("Кошелек не найден.");
            }
        }

        public async Task<decimal> ConversionCoinAsync(string fromCurrency, string toCurrency, decimal amount, CancellationToken cancellationToken)
        {
            var wallets = await _walletStorage.GetAllWalletsAsync(cancellationToken);
            var wallet = wallets.FirstOrDefault(); // Предполагается, что есть только один кошелек

            if (wallet != null)
            {
                var coinFrom = await _coinStorage.GetCoinAsync(fromCurrency, cancellationToken);
                var coinTo = await _coinStorage.GetCoinAsync(toCurrency, cancellationToken);

                if (coinFrom == null || coinTo == null)
                {
                    throw new WalletException("Неизвестная валюта для конвертации.");
                }

                var coinAmount = await _walletStorage.GetCoinAmountAsync(wallet.id, fromCurrency, cancellationToken);

                if (amount < 0)
                {
                    throw new WalletException("Сумма должна быть положительным числом.");
                }

                if (coinAmount == null || coinAmount.Amount < amount)
                {
                    throw new WalletException("У вас недостаточно денег на кошельке.");
                }

                decimal convertedAmount = amount * (coinFrom.Course / coinTo.Course);
                coinAmount.Amount -= amount;
                await _walletStorage.UpdateCoinAmountAsync(coinAmount, cancellationToken);

                var targetCoinAmount = await _walletStorage.GetCoinAmountAsync(wallet.id, toCurrency, cancellationToken);
                if (targetCoinAmount != null)
                {
                    targetCoinAmount.Amount += convertedAmount;
                    await _walletStorage.UpdateCoinAmountAsync(targetCoinAmount, cancellationToken);
                }
                else
                {
                    targetCoinAmount = new CoinAmount(convertedAmount, coinTo.Name, wallet.id);
                    await _walletStorage.AddCoinAmountAsync(targetCoinAmount, cancellationToken);
                }

                await _transactionStorage.AddTransactionAsync(new Transaction(Guid.NewGuid(), "Конвертация", convertedAmount, DateTime.UtcNow), cancellationToken);

                return convertedAmount;
            }

            throw new WalletException("Кошелек не найден.");
        }

        public async Task<Dictionary<string, decimal>> GetBalanceAsync(CancellationToken cancellationToken)
        {
            var wallets = await _walletStorage.GetAllWalletsAsync(cancellationToken);
            var balance = new Dictionary<string, decimal>();

            if (wallets.Any())
            {
                var wallet = wallets.First(); // Предполагается, что есть только один кошелек
                var coinAmounts = await _walletStorage.GetCoinAmountsIdAsync(wallet.id, cancellationToken);

                foreach (var coinAmount in coinAmounts)
                {
                    balance[coinAmount.CoinName] = coinAmount.Amount;
                }
            }

            return balance;
        }

        public async Task<List<Transaction>> GetHistoryAsync(CancellationToken cancellationToken)
        {
            return await _transactionStorage.GetAllTransactionsAsync(cancellationToken);
        }
    }
}
