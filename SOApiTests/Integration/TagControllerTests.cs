using FluentAssertions;
using NSubstitute;
using SOApiProject.Controllers;
using SOApiProject.Data;
using SOApiProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SOApiTests.Integration
{
    public class TagsControllerIntegrationTests
    {
        private readonly IMongoService _mongoService;
        private readonly IDatabaseInitializer _databaseInitializer;
        private readonly TagsController _controller;

        public TagsControllerIntegrationTests()
        {
            _mongoService = Substitute.For<IMongoService>();
            _databaseInitializer = Substitute.For<IDatabaseInitializer>();
            _controller = new TagsController(_mongoService, _databaseInitializer);
        }

        [Fact]
        public async Task GetTagsShare_ReturnsExpectedResult()
        {
            //Todo dokoncyc test z pagedlistmoodel
            // Arrange
            var expectedTags = new List<TagModel>
            {
                new TagModel { Name = "C#", Count = 100 },
                new TagModel { Name = "Java", Count = 50 }
            };

            _mongoService.GetSortedTags("Name",true,100).Returns(new List<PagedTagModel>
            {
                new PagedTagModel
                {
                    Content = expectedTags,
                    PageNumber = 1
                },
                new PagedTagModel()
            });

            // Act
            var result =  _controller.GetTagsShare().Result.Value;
            
            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainKey("C#");
            result.Should().ContainKey("Java");
            result["C#"].Should().BeApproximately(66.67, 0.01);
            result["Java"].Should().BeApproximately(33.33, 0.01);
        }

        // [Fact]
        // public async Task GetTags_ReturnsPagedResult()
        // {
        //     // Arrange
        //     var expectedTags = Enumerable.Range(1, 200)
        //         .Select(i => new TagModel { Name = $"Tag{i}", Count = i })
        //         .ToList();
        //
        //     _mongoService.GetTagsAsync("Name", true).Returns(expectedTags);
        //
        //     // Act
        //     var result =  _controller.GetTags();
        //
        //     // Assert
        //     result.Should().HaveCount(2);
        //     result[0].Content.Should().HaveCount(100);
        //     result[1].Content.Should().HaveCount(100);
        //     result[0].PageNumber.Should().Be(1);
        //     result[1].PageNumber.Should().Be(2);
        //     result[0].Content[0].Name.Should().Be("Tag1");
        //     result[0].Content[99].Name.Should().Be("Tag100");
        //     result[1].Content[0].Name.Should().Be("Tag101");
        //     result[1].Content[99].Name.Should().Be("Tag200");
        // }

        [Fact]
        public async Task RepopulateDatabase_DropsCollectionAndInitializesDb()
        {
            // Act
            await _controller.RepopulateDatabase(2000);

            // Assert
            await _mongoService.Received(1).DropCollection();
            await _databaseInitializer.Received(1).InitDb(Arg.Any<int>());
        }
    }
}
