namespace SocialNetwork.Test.Repositories;

public class DirectMessageRepositoryTests
{
    private readonly SocialNetworkDbContext _db;
    private readonly DirectMessageRepository _sut;

    public DirectMessageRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SocialNetworkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _db = new SocialNetworkDbContext(options);
        _sut = new DirectMessageRepository(_db);
    }

    [Fact]
    public async Task AddAsync_SavesDirectMessageToDatabase()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var content = "Test message content";
        var messageId = Guid.NewGuid();
        var directMessage = new DirectMessage
        {
            Id = messageId,
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _sut.AddAsync(directMessage);

        // Assert
        var saved = await _db.DirectMessages.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal(messageId, saved.Id);
        Assert.Equal(senderId, saved.SenderId);
        Assert.Equal(receiverId, saved.ReceiverId);
        Assert.Equal(content, saved.Content);
        Assert.NotEqual(default(DateTime), saved.CreatedAt);
    }

    [Fact]
    public async Task AddAsync_SavesMultipleDirectMessagesToDatabase()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        
        var message1 = new DirectMessage
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = "First message",
            CreatedAt = DateTime.UtcNow
        };

        var message2 = new DirectMessage
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = "Second message",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _sut.AddAsync(message1);
        await _sut.AddAsync(message2);

        // Assert
        var savedMessages = await _db.DirectMessages.ToListAsync();
        Assert.Equal(2, savedMessages.Count);
        
        var savedMessage1 = savedMessages.FirstOrDefault(m => m.Content == "First message");
        var savedMessage2 = savedMessages.FirstOrDefault(m => m.Content == "Second message");
        
        Assert.NotNull(savedMessage1);
        Assert.Equal(senderId, savedMessage1.SenderId);
        Assert.Equal(receiverId, savedMessage1.ReceiverId);
        
        Assert.NotNull(savedMessage2);
        Assert.Equal(senderId, savedMessage2.SenderId);
        Assert.Equal(receiverId, savedMessage2.ReceiverId);
    }
}
