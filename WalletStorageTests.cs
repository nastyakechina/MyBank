using Moq;
using NUnit.Framework;
using Storage;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Storage.Tests
{
    [TestFixture]
    public class WalletStorageTests
    {
        private Mock<IDatabaseStorage<Wallet>> _mockWalletRepository;
        private Mock<IDatabaseStorage<CoinAmount>> _mockCoinAmountRepository;
        private WalletStorage _walletStorage;

        [SetUp]
        public void Setup()
        {
            _mockWalletRepository = new Mock<IDatabaseStorage<Wallet>>();
            _mockCoinAmountRepository = new Mock<IDatabaseStorage<CoinAmount>>();

            _walletStorage = new WalletStorage(_mockWalletRepository.Object, _mockCoinAmountRepository.Object);
        }

        [Test]
        public async Task AddWalletAsync_AddsWallet()
        {
            // Arrange
            var wallet = new Wallet(Guid.NewGuid(), "My Wallet");

            _mockWalletRepository
                .Setup(repo => repo.AddAsync(wallet, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _walletStorage.AddWalletAsync(wallet, CancellationToken.None);

            // Assert
            _mockWalletRepository.Verify(repo => repo.AddAsync(wallet, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllWalletsAsync_ReturnsListOfWallets()
        {
            // Arrange
            var wallets = new List<Wallet>
            {
                new Wallet(Guid.NewGuid(), "Wallet 1"),
                new Wallet(Guid.NewGuid(), "Wallet 2")
            };

            _mockWalletRepository
                .Setup(repo => repo.GetListAsync("1 = 1", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(wallets);

            // Act
            var result = await _walletStorage.GetAllWalletsAsync(CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Wallet 1", result[0].Name);
        }

        [Test]
        public async Task UpdateWalletAsync_UpdatesWallet()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var updatedWallet = new Wallet(walletId, "Updated Wallet");

            _mockWalletRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<object>(), updatedWallet, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _walletStorage.UpdateWalletAsync(walletId, updatedWallet, CancellationToken.None);

            // Assert
            _mockWalletRepository.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<object>(), updatedWallet, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetCoinAmountAsync_ReturnsCoinAmount()
        {
            // Arrange
            var walletId = Guid.NewGuid();
            var coinAmount = new CoinAmount(100m, "USD", walletId);
            var coinAmounts = new List<CoinAmount> { coinAmount };

            _mockCoinAmountRepository
                .Setup(repo => repo.GetListAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(coinAmounts);

            // Act
            var result = await _walletStorage.GetCoinAmountAsync(walletId, "USD", CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("USD", result.CoinName);
            Assert.AreEqual(100m, result.Amount);
        }

        [Test]
        public async Task GetCoinAmountAsync_ReturnsNullIfNotFound()
        {
            // Arrange
            var walletId = Guid.NewGuid();

            _mockCoinAmountRepository
                .Setup(repo => repo.GetListAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CoinAmount>());

            // Act
            var result = await _walletStorage.GetCoinAmountAsync(walletId, "USD", CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddCoinAmountAsync_AddsCoinAmount()
        {
            // Arrange
            var coinAmount = new CoinAmount(100m, "USD", Guid.NewGuid());

            _mockCoinAmountRepository
                .Setup(repo => repo.AddAsync(coinAmount, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _walletStorage.AddCoinAmountAsync(coinAmount, CancellationToken.None);

            // Assert
            _mockCoinAmountRepository.Verify(repo => repo.AddAsync(coinAmount, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateCoinAmountAsync_UpdatesCoinAmount()
        {
            // Arrange
            var coinAmount = new CoinAmount(100m, "USD", Guid.NewGuid());

            _mockCoinAmountRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<object>(), coinAmount, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _walletStorage.UpdateCoinAmountAsync(coinAmount, CancellationToken.None);

            // Assert
            _mockCoinAmountRepository.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<object>(), coinAmount, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}







/*using Moq;
using NUnit.Framework;
using Storage;
using Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Storage.Tests
{
    [TestFixture]
    public class WalletStorageTests
    {
        private Mock<IStorageFile<Wallet>> _walletStorageFileMock;
        private Mock<IStorageFile<CoinAmount>> _coinAmountStorageFileMock;
        private WalletStorage _walletStorage;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _walletStorageFileMock = new Mock<IStorageFile<Wallet>>();
            _coinAmountStorageFileMock = new Mock<IStorageFile<CoinAmount>>();
            _walletStorage = new WalletStorage(_walletStorageFileMock.Object, _coinAmountStorageFileMock.Object);
            _cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task GetAllWalletsAsync_ReturnsWalletList()
        {
            // Arrange
            var wallets = new List<Wallet>
            {
                new Wallet(Guid.NewGuid(), "Test Wallet")
            };
            _walletStorageFileMock
                .Setup(file => file.GetAllAsync(_cancellationToken))
                .ReturnsAsync(wallets);

            // Act
            var result = await _walletStorage.GetAllWalletsAsync(_cancellationToken);

            // Assert
            Assert.AreEqual(wallets, result);
            _walletStorageFileMock.Verify(file => file.GetAllAsync(_cancellationToken), Times.Once);
        }
    }
}*/