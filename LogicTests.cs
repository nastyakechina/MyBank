using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Models;
using Presenter;
using Storage;

namespace Tests
{
    [TestFixture]
    public class LogicTests
    {
        private Mock<ICoinStorage> _mockCoinStorage;
        private Mock<IWalletStorage> _mockWalletStorage;
        private Mock<ITransactionStorage> _mockTransactionStorage;
        private Logic _logic;

        [SetUp]
        public void Setup()
        {
            _mockCoinStorage = new Mock<ICoinStorage>();
            _mockWalletStorage = new Mock<IWalletStorage>();
            _mockTransactionStorage = new Mock<ITransactionStorage>();

            _logic = new Logic(_mockCoinStorage.Object, _mockWalletStorage.Object, _mockTransactionStorage.Object);
        }

        [Test]
        public async Task AddNewCoinAsync_ValidCoin_AddsCoin()
        {
            // Arrange
            var coin = new Coin("USD", 74.5m);
            _mockCoinStorage
                .Setup(cs => cs.GetCoinAsync("USD", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Coin)null); // Валюта еще не добавлена

            // Act
            await _logic.AddNewCoinAsync(coin, CancellationToken.None);

            // Assert
            _mockCoinStorage.Verify(cs => cs.AddCoinAsync(coin, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void AddNewCoinAsync_DuplicateCoin_ThrowsException()
        {
            // Arrange
            var coin = new Coin("USD", 74.5m);
            _mockCoinStorage
                .Setup(cs => cs.GetCoinAsync("USD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(coin); // Валюта уже существует

            // Act & Assert
            Assert.ThrowsAsync<WalletException>(() => _logic.AddNewCoinAsync(coin, CancellationToken.None));
        }

        [Test]
        public async Task DepositWalletAsync_ValidCoinAndAmount_UpdatesWallet()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var wallet = new Wallet(walletId, "Кошелек");
            _mockWalletStorage
                .Setup(ws => ws.GetAllWalletsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Wallet> { wallet });

            var coinAmount = new CoinAmount(100, "USD", walletId);
            _mockWalletStorage
                .Setup(ws => ws.GetCoinAmountAsync(walletId, "USD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(coinAmount);

            // Act
            await _logic.DepositWalletAsync("USD", 50, CancellationToken.None);

            // Assert
            Assert.AreEqual(150, coinAmount.Amount);
            _mockWalletStorage.Verify(ws => ws.UpdateCoinAmountAsync(coinAmount, It.IsAny<CancellationToken>()), Times.Once);
            _mockTransactionStorage.Verify(ts => ts.AddTransactionAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void DepositWalletAsync_NegativeAmount_ThrowsException()
        {
            // Arrange
            _mockWalletStorage
                .Setup(ws => ws.GetAllWalletsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Wallet> { new Wallet(Guid.NewGuid(), "Кошелек") });

            // Act & Assert
            Assert.ThrowsAsync<WalletException>(() => _logic.DepositWalletAsync("USD", -50, CancellationToken.None));
        }

        [Test]
        public void ConversionCoinAsync_InsufficientFunds_ThrowsException()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var wallet = new Wallet(walletId, "Кошелек");
            _mockWalletStorage
                .Setup(ws => ws.GetAllWalletsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Wallet> { wallet });

            var coinFrom = new Coin("USD", 74.5m);
            _mockCoinStorage.Setup(cs => cs.GetCoinAsync("USD", It.IsAny<CancellationToken>())).ReturnsAsync(coinFrom);

            var coinAmount = new CoinAmount(10, "USD", walletId); // Недостаточно средств
            _mockWalletStorage
                .Setup(ws => ws.GetCoinAmountAsync(walletId, "USD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(coinAmount);

            // Act & Assert
            Assert.ThrowsAsync<WalletException>(() => _logic.ConversionCoinAsync("USD", "EUR", 50, CancellationToken.None));
        }
    }
}



/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Models;
using Storage;
using Presenter;

namespace Presenter.Tests
{
    public class LogicTests
    {
        private Mock<ICoinStorage> _coinStorageMock;
        private Mock<IWalletStorage> _walletStorageMock;
        private Mock<ITransactionStorage> _transactionStorageMock;
        private Logic _logic;

        [SetUp]
        public void Setup()
        {
            _coinStorageMock = new Mock<ICoinStorage>();
            _walletStorageMock = new Mock<IWalletStorage>();
            _transactionStorageMock = new Mock<ITransactionStorage>();

            _walletStorageMock
                .Setup(w => w.AddWalletAsync(It.IsAny<Wallet>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _logic = new Logic(_coinStorageMock.Object, _walletStorageMock.Object, _transactionStorageMock.Object);
        }

        [Test]
        public async Task AddNewCoinAsync_ShouldAddCoin_WhenCoinIsValidAndDoesNotExist()
        {
            // Arrange
            var coin = new Coin("USD", 1.0m);
            _coinStorageMock.Setup(cs => cs.GetCoinAsync(coin.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Coin)null);
            _coinStorageMock.Setup(cs => cs.AddCoinAsync(coin, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _logic.AddNewCoinAsync(coin, CancellationToken.None);

            // Assert
            _coinStorageMock.Verify(cs => cs.AddCoinAsync(coin, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void AddNewCoinAsync_ShouldThrow_WhenCoinAlreadyExists()
        {
            // Arrange
            var coin = new Coin("USD", 1.0m);
            _coinStorageMock.Setup(cs => cs.GetCoinAsync(coin.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(coin);

            // Act & Assert
            Assert.ThrowsAsync<WalletException>(() => _logic.AddNewCoinAsync(coin, CancellationToken.None));
        }

        [Test]
        public async Task DepositWalletAsync_ShouldIncreaseCoinAmount_WhenCoinExists()
        {
            // Arrange
            var wallet = new Wallet(Guid.NewGuid(), "Test Wallet");
            var coin = new Coin("USD", 1.0m);
            var coinAmount = new CoinAmount(100, coin, wallet.id);

            _walletStorageMock.Setup(ws => ws.GetAllWalletsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Wallet> { wallet });
            _walletStorageMock.Setup(ws => ws.GetCoinAmountAsync(wallet.id, coin.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(coinAmount);
            _walletStorageMock.Setup(ws => ws.UpdateCoinAmountAsync(It.IsAny<CoinAmount>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _transactionStorageMock.Setup(ts => ts.AddTransactionAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _logic.DepositWalletAsync(coin.Name, 50, CancellationToken.None);

            // Assert
            Assert.AreEqual(150, coinAmount.Amount);
            _walletStorageMock.Verify(ws => ws.UpdateCoinAmountAsync(It.Is<CoinAmount>(ca => ca.Amount == 150), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void DepositWalletAsync_ShouldThrow_WhenAmountIsNegative()
        {
            // Arrange
            var wallet = new Wallet(Guid.NewGuid(), "Test Wallet");
            _walletStorageMock.Setup(ws => ws.GetAllWalletsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Wallet> { wallet });

            // Act & Assert
            Assert.ThrowsAsync<WalletException>(() => _logic.DepositWalletAsync("USD", -50, CancellationToken.None));
        }

        [Test]
        public async Task ConversionCoinAsync_ShouldConvert_WhenSufficientBalanceExists()
        {
            // Arrange
            var wallet = new Wallet(Guid.NewGuid(), "Test Wallet");
            var coinFrom = new Coin("USD", 1.0m);
            var coinTo = new Coin("EUR", 0.85m);
            var coinAmount = new CoinAmount(100, coinFrom, wallet.id);

            _walletStorageMock.Setup(ws => ws.GetAllWalletsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Wallet> { wallet });
            _coinStorageMock.Setup(cs => cs.GetCoinAsync("USD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(coinFrom);
            _coinStorageMock.Setup(cs => cs.GetCoinAsync("EUR", It.IsAny<CancellationToken>()))
                .ReturnsAsync(coinTo);
            _walletStorageMock.Setup(ws => ws.GetCoinAmountAsync(wallet.id, "USD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(coinAmount);
            _walletStorageMock.Setup(ws => ws.UpdateCoinAmountAsync(It.IsAny<CoinAmount>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _transactionStorageMock.Setup(ts => ts.AddTransactionAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var convertedAmount = await _logic.ConversionCoinAsync("USD", "EUR", 50, CancellationToken.None);

            // Assert
            Assert.AreEqual(50 * (1.0m / 0.85m), convertedAmount);
            _walletStorageMock.Verify(ws => ws.UpdateCoinAmountAsync(It.Is<CoinAmount>(ca => ca.Amount == 50), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void ConversionCoinAsync_ShouldThrow_WhenInsufficientBalance()
        {
            // Arrange
            var wallet = new Wallet(Guid.NewGuid(), "Test Wallet");
            var coinFrom = new Coin("USD", 1.0m);
            var coinAmount = new CoinAmount(10, coinFrom, wallet.id);

            _walletStorageMock.Setup(ws => ws.GetAllWalletsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Wallet> { wallet });
            _coinStorageMock.Setup(cs => cs.GetCoinAsync("USD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(coinFrom);
            _walletStorageMock.Setup(ws => ws.GetCoinAmountAsync(wallet.id, "USD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(coinAmount);

            // Act & Assert
            Assert.ThrowsAsync<WalletException>(() => _logic.ConversionCoinAsync("USD", "EUR", 50, CancellationToken.None));
        }
    }
}*/
