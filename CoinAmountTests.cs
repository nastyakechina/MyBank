using NUnit.Framework;
using Models;
using System;

namespace Models.Tests
{
    [TestFixture]
    public class CoinAmountTests
    {
        [Test]
        public void Constructor_WithParameters_InitializesPropertiesCorrectly()
        {
            // Arrange
            decimal amount = 100.5m;
            string coinName = "USD";
            Guid walletId = Guid.NewGuid();

            // Act
            var coinAmount = new CoinAmount(amount, coinName, walletId);

            // Assert
            Assert.AreEqual(amount, coinAmount.Amount, "Amount was not initialized correctly.");
            Assert.AreEqual(coinName, coinAmount.CoinName, "CoinName was not initialized correctly.");
            Assert.AreEqual(walletId, coinAmount.WalletId, "WalletId was not initialized correctly.");
        }

        [Test]
        public void DefaultConstructor_AllowsPropertySetAndGet()
        {
            // Arrange
            decimal amount = 50m;
            string coinName = "EUR";
            Guid walletId = Guid.NewGuid();

            // Act
            var coinAmount = new CoinAmount();
            coinAmount.Amount = amount;
            coinAmount.CoinName = coinName;
            coinAmount.WalletId = walletId;

            // Assert
            Assert.AreEqual(amount, coinAmount.Amount, "Amount property set/get failed.");
            Assert.AreEqual(coinName, coinAmount.CoinName, "CoinName property set/get failed.");
            Assert.AreEqual(walletId, coinAmount.WalletId, "WalletId property set/get failed.");
        }
    }
}



/*using NUnit.Framework;
using System;

namespace Models.Tests
{
    public class CoinAmountTests
    {
        [Test]
        public void Constructor_ShouldInitializeAmountCoinAndWalletId()
        {
            // Arrange
            var coin = new Coin("USD", 1.0m);
            decimal amount = 100;
            Guid walletId = Guid.NewGuid();

            // Act
            var coinAmount = new CoinAmount(amount, coin, walletId);

            // Assert
            Assert.AreEqual(amount, coinAmount.Amount, "Amount was not initialized correctly.");
            Assert.AreEqual(coin, coinAmount.Coin, "Coin was not initialized correctly.");
            Assert.AreEqual(walletId, coinAmount.WalletId, "WalletId was not initialized correctly.");
        }

        [Test]
        public void Amount_SetValue_ShouldUpdateAmount()
        {
            // Arrange
            var coin = new Coin("USD", 1.0m);
            Guid walletId = Guid.NewGuid();
            var coinAmount = new CoinAmount(100, coin, walletId);

            // Act
            coinAmount.Amount = 200;

            // Assert
            Assert.AreEqual(200, coinAmount.Amount, "Amount did not update correctly.");
        }

        [Test]
        public void Coin_SetValue_ShouldUpdateCoin()
        {
            // Arrange
            var initialCoin = new Coin("USD", 1.0m);
            var newCoin = new Coin("EUR", 0.85m);
            Guid walletId = Guid.NewGuid();
            var coinAmount = new CoinAmount(100, initialCoin, walletId);

            // Act
            coinAmount.Coin = newCoin;

            // Assert
            Assert.AreEqual(newCoin, coinAmount.Coin, "Coin did not update correctly.");
        }

        [Test]
        public void WalletId_SetValue_ShouldUpdateWalletId()
        {
            // Arrange
            var coin = new Coin("USD", 1.0m);
            Guid initialWalletId = Guid.NewGuid();
            Guid newWalletId = Guid.NewGuid();
            var coinAmount = new CoinAmount(100, coin, initialWalletId);

            // Act
            coinAmount.WalletId = newWalletId;

            // Assert
            Assert.AreEqual(newWalletId, coinAmount.WalletId, "WalletId did not update correctly.");
        }
    }
}*/
