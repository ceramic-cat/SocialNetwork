using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetwork.Entity.Models;
using SocialNetwork.API.Controllers;
using SocialNetwork.API.Models;
using SocialNetwork.Repository.Services;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace SocialNetwork.Test.Controllers
{
    public class TimelineControllerTests
    {
        private readonly Mock<ITimelineService> _timelineServiceMock;
        private readonly TimelineController _sut;

        public TimelineControllerTests()
        {
            _timelineServiceMock = new Mock<ITimelineService>();
            _sut = new TimelineController(_timelineServiceMock.Object);
        }

        [Fact]
        public async Task GetTimeline_ReturnsOkWithMappedPosts()
        {
            //arrange
            var userId = Guid.NewGuid();
            var posts = new List<Post>
            {
                new Post { Id = Guid.NewGuid(), Content = "Post 1", CreatedAt = DateTime.UtcNow },
                new Post { Id = Guid.NewGuid(), Content = "Post 2", CreatedAt = DateTime.UtcNow }
            };

            _timelineServiceMock
                .Setup(s => s.GetPostsByUserIdAsync(userId))
                .ReturnsAsync(posts);

            //act
            var result = await _sut.GetTimeline(userId);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var dtos = Assert.IsType<List<PostDto>>(okResult.Value);
            Assert.Equal(posts.Count, dtos.Count);

            Assert.Equal(posts[0].Id, dtos[0].Id);
            Assert.Equal(posts[0].SenderId, dtos[0].SenderId);
            Assert.Equal(posts[0].ReceiverId, dtos[0].ReceiverId);
            Assert.Equal(posts[0].Content, dtos[0].Content);
            Assert.Equal(posts[0].CreatedAt, dtos[0].CreatedAt);

        }

    }
}
