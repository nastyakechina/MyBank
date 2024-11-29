using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Storage;

[TestFixture]
public class TransactionStorageTests
{
    private Mock<IDatabaseStorage<Transaction>> _mockDatabaseStorage;
    private TransactionStorage _transactionStorage;

    [SetUp]
    public void Setup()
    {
        _mockDatabaseStorage = new Mock<IDatabaseStorage<Transaction>>();
        
        _transactionStorage = new TransactionStorage(_mockDatabaseStorage.Object);
    }

    [Test]
    public async Task GetAllTransactionsAsync_ReturnsAllTransactions()
    {
        // Arrange
        var fakeTransactions = new List<Transaction>
        {
            new Transaction(Guid.NewGuid(), "Deposit", 100.0m, DateTime.UtcNow),
            new Transaction(Guid.NewGuid(), "Conversion", 50.0m, DateTime.UtcNow)
        };

        _mockDatabaseStorage
            .Setup(db => db.GetListAsync("1 = 1", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeTransactions);

        // Act
        var result = await _transactionStorage.GetAllTransactionsAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Deposit", result[0].Type);
        Assert.AreEqual(100.0m, result[0].Amount);
        _mockDatabaseStorage.Verify(db => db.GetListAsync("1 = 1", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AddTransactionAsync_AddsTransaction()
    {
        // Arrange
        var transaction = new Transaction(Guid.NewGuid(), "Deposit", 200.0m, DateTime.UtcNow);

        _mockDatabaseStorage
            .Setup(db => db.AddAsync(transaction, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _transactionStorage.AddTransactionAsync(transaction, CancellationToken.None);

        // Assert
        _mockDatabaseStorage.Verify(db => db.AddAsync(transaction, It.IsAny<CancellationToken>()), Times.Once);
    }
}
