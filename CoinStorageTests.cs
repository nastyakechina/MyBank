using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Storage;

[TestFixture]
public class CoinStorageTests
{
    private Mock<IDatabaseStorage<Coin>> _mockDatabaseStorage;
    private CoinStorage _coinStorage;

    [SetUp]
    public void Setup()
    {
        _mockDatabaseStorage = new Mock<IDatabaseStorage<Coin>>();
        _coinStorage = new CoinStorage(_mockDatabaseStorage.Object);
    }

    [Test]
    public async Task AddCoinAsync_AddsCoin()
    {
        // Arrange
        var coin = new Coin("USD", 100);
        
        _mockDatabaseStorage
            .Setup(storage => storage.AddAsync(It.IsAny<Coin>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);  // Мокаем асинхронную операцию

        // Act
        await _coinStorage.AddCoinAsync(coin, CancellationToken.None);

        // Assert
        _mockDatabaseStorage.Verify(storage => storage.AddAsync(It.IsAny<Coin>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetCoinAsync_ReturnsCoin()
    {
        // Arrange
        var coin = new Coin("USD", 100);
        var name = "USD";
        
        _mockDatabaseStorage
            .Setup(storage => storage.GetListAsync("name = @name", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Coin> { coin });

        // Act
        var result = await _coinStorage.GetCoinAsync(name, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("USD", result.Name);
        _mockDatabaseStorage.Verify(storage => storage.GetListAsync("name = @name", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetCoinAsync_ReturnsNullIfCoinNotFound()
    {
        // Arrange
        var name = "USD";
        
        _mockDatabaseStorage
            .Setup(storage => storage.GetListAsync("name = @name", It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Coin>());

        // Act
        var result = await _coinStorage.GetCoinAsync(name, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }
}
