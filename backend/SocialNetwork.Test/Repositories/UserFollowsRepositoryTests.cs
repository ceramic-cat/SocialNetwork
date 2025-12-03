using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Test.Repositories;

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




}
