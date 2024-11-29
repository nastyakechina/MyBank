using System;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Storage;
using Presenter;
using View;
using Npgsql;
using Dapper;

namespace Bank
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=nastya02032005;Database=postgres";
            
            var walletRepository = new DatabaseStorage<Wallet>(connectionString, "wallets");
            var coinAmountRepository = new DatabaseStorage<CoinAmount>(connectionString, "coin_amounts");
            var transactionRepository = new DatabaseStorage<Transaction>(connectionString, "transactions");
            var coinRepository = new DatabaseStorage<Coin>(connectionString, "coins");
            
            var walletStorage = new WalletStorage(walletRepository, coinAmountRepository);
            var transactionStorage = new TransactionStorage(transactionRepository);
            var coinStorage = new CoinStorage(coinRepository);
            
            var menu = new Menu(coinStorage, walletStorage,transactionStorage);
            
            await menu.StartMenuAsync(CancellationToken.None);
        }
    }
}

                       

         
            /*
            var coinStorage = new CoinStorage();
            var walletStorage = new WalletStorage();
            var transactionStorage = new TransactionStorage();
            
            var menu = new Menu(coinStorage, walletStorage, transactionStorage);
            
            await menu.StartMenuAsync(CancellationToken.None);
            
            //await menu.StartMenuAsync(new CancellationToken(false));
            
            // using var conn = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=nastya02032005;Database=postgres");
            // conn.Open();
            // using (var cmd = new NpgsqlCommand("SELECT NOW()",conn))
            // {
            //     var now = cmd.ExecuteScalar();
            //     Console.WriteLine($"Current time {now}");
            // }*/