using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Test.Repositories;

// IDisposable disposes of the database after each test.
public class UserFollowsRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SocialNetworkDbContext _db;
    private readonly UserFollowsRepository _repository;

    public UserFollowsRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<SocialNetworkDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new SocialNetworkDbContext(options);
        _db.Database.EnsureCreated();

        _repository = new UserFollowsRepository(_db);
    }


    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task AddAsync_SavesFollowToDatabase()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        // Act
        await _repository.AddAsync(followerId, followeeId);

        // Assert
        var saved = await _db.UserFollows.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal(followeeId, saved.FolloweeId);
        Assert.Equal(followerId, saved.FollowerId);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueIfFollows()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        // Act
        await _repository.AddAsync(followerId, followeeId);

        var result = await _repository.ExistsAsync(followerId, followeeId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WhenNotFollowing_ReturnsFalse()
    {        
            // Arrange
            var followerId = Guid.NewGuid();
            var followeeId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(followerId, followeeId);

            // Assert
            Assert.False(result);

        }

    [Fact]
    public async Task DeleteAsync_DeletesFollowFromDatabase()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followeeId = Guid.NewGuid();

        // Act
        await _repository.AddAsync(followerId, followeeId);
        var successfulSave = await _repository.ExistsAsync(followerId, followeeId);

        await _repository.DeleteAsync(followerId, followeeId);
        var sucessfulDelete = await _repository.ExistsAsync(followerId, followeeId);

        // Assert
        Assert.True(successfulSave);
        Assert.False(sucessfulDelete);
    }

}
