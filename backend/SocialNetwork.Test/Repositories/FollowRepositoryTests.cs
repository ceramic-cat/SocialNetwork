namespace SocialNetwork.Test.Repositories;

// IDisposable disposes of the database after each test.
public class FollowRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SocialNetworkDbContext _db;
    private readonly FollowRepository _repository;

    public FollowRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<SocialNetworkDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new SocialNetworkDbContext(options);
        _db.Database.EnsureCreated();

        _repository = new FollowRepository(_db);
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
        var saved = await _db.Follows.FirstOrDefaultAsync();
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

    [Fact]
    public async Task GetFollowsAsync_ReturnsFolloweeIds()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followee1 = Guid.NewGuid();
        var followee2 = Guid.NewGuid();
        var followee3 = Guid.NewGuid();

        _db.Follows.AddRange(
            new Follow { FollowerId = followerId, FolloweeId = followee1 },
            new Follow { FollowerId = followerId, FolloweeId = followee2 },
            new Follow { FollowerId = followerId, FolloweeId = followee3 }
        );
        await _db.SaveChangesAsync();

        // Act
        var result = await _repository.GetFollowsAsync(followerId);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Contains(followee1, result);
        Assert.Contains(followee2, result);
        Assert.Contains(followee3, result);
    }

    [Fact]
    public async Task GetFollowsAsync_WhenUserFollowsNobody_ReturnsEmptyArray()
    {
        // Arrange
        var followerId = Guid.NewGuid();

        // Act
        var result = await _repository.GetFollowsAsync(followerId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFollowsAsync_OnlyReturnsFolloweesForSpecifiedUser()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var followee1 = Guid.NewGuid();
        var followee2 = Guid.NewGuid();

        _db.Follows.AddRange(
            new Follow { FollowerId = user1, FolloweeId = followee1 },
            new Follow { FollowerId = user2, FolloweeId = followee2 });
        await _db.SaveChangesAsync();

        // Act
        var result = await _repository.GetFollowsAsync(user1);

        // Assert
        Assert.Contains(followee1, result);
        Assert.DoesNotContain(followee2, result);
    }

    [Fact]
    public async Task GetFollowsWithUserInfoAsync_ReturnsUserIdsAndUsernames()
    {
        // Arrange
        var followerId = Guid.NewGuid();

        var alice = new User { Id = Guid.NewGuid(), Username = "alice" };
        var bob = new User { Id = Guid.NewGuid(), Username = "bob" };

        _db.Users.AddRange(alice, bob);
        _db.Follows.AddRange(
            new Follow { FollowerId = followerId, FolloweeId = alice.Id },
            new Follow { FollowerId = followerId, FolloweeId = bob.Id }
        );
        await _db.SaveChangesAsync();

        // Act
        var result = await _repository.GetFollowsWithUserInfoAsync(followerId);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Contains(result, r => r.Id == alice.Id && r.Username == "alice");
        Assert.Contains(result, r => r.Id == bob.Id && r.Username == "bob");
    }

    [Fact]
    public async Task GetFollowsWithUserInfoAsync_WhenUserFollowsNobody_ReturnsEmptyArray()
    {
        // Arrange
        var followerId = Guid.NewGuid();

        // Act
        var result = await _repository.GetFollowsWithUserInfoAsync(followerId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFollowsWithUserInfoAsync_OnlyReturnsFolloweesForSpecifiedUser()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();

        var alice = new User { Id = Guid.NewGuid(), Username = "alice" };
        var bob = new User { Id = Guid.NewGuid(), Username = "bob" };

        _db.Users.AddRange(alice, bob);
        _db.Follows.AddRange(
            new Follow { FollowerId = user1, FolloweeId = alice.Id },
            new Follow { FollowerId = user2, FolloweeId = bob.Id }
        );
        await _db.SaveChangesAsync();

        // Act
        var result = await _repository.GetFollowsWithUserInfoAsync(user1);

        // Assert
        Assert.Single(result);
        Assert.Equal("alice", result[0].Username);
        Assert.DoesNotContain(result, r => r.Username == "bob");
    }

    [Fact]
    public async Task GetFollowsWithUserInfoAsync_DoesNotIncludeFollower()
    {
        // Arrange
        var follower = new User { Id = Guid.NewGuid(), Username = "follower" };
        var followee = new User { Id = Guid.NewGuid(), Username = "followee" };

        _db.Users.AddRange(follower, followee);
        _db.Follows.Add(new Follow { FollowerId = follower.Id, FolloweeId = followee.Id });
        await _db.SaveChangesAsync();

        // Act
        var result = await _repository.GetFollowsWithUserInfoAsync(follower.Id);

        // Assert
        Assert.Single(result);
        Assert.Equal("followee", result[0].Username);
        Assert.DoesNotContain(result, r => r.Username == "follower");
    }

}
