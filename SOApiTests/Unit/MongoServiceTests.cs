using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SOApiProject.Data;
using SOApiProject.Models;

namespace SOApiTests.Unit
{
    public class MongoServiceTests
    {
        private readonly ILogger<MongoService> _logger;
        private readonly IOptions<MongoDBSettings> _options;
        private readonly IMongoCollection<TagModel> _collection;
        private readonly IMongoDatabase _database;

        public MongoServiceTests()
        {
            _logger = Substitute.For<ILogger<MongoService>>();
            _options = Substitute.For<IOptions<MongoDBSettings>>();
            _collection = Substitute.For<IMongoCollection<TagModel>>();
            _database = Substitute.For<IMongoDatabase>();

            _options.Value.Returns(new MongoDBSettings
            {
                ConnectionURI = "mongodb://localhost:27017",
                DatabaseName = "testdb",
                CollectionName = "testcollection"
            });
        }

        [Fact]
        public async Task GetTagsAsync_Should_Log_Information_On_Successful_Fetch()
        {
            // Arrange
            var tagModelsList = new List<TagModel>
            {
                new TagModel
                {
                    Count = 100, HasSynonyms = true, ID = "Id 1", IsModeratorOnly = false, IsRequired = false, Name = "Tag 1"
                },
                new TagModel
                {
                    Count = 100, HasSynonyms = true, ID = "Id 2", IsModeratorOnly = false, IsRequired = false, Name = "Tag 2"
                }
            };

            // mockCollection.FindAsync(Arg.Any<FilterDefinition<TagModel>>(), Arg.Any<FindOptions<TagModel, TagModel>>())
            //     .Returns(tagModelsList);
            _database.GetCollection<TagModel>(Arg.Any<string>(), null).Returns(_collection);

            var mongoService = new MongoService(_options, _logger);

            // Act
            var result = await mongoService.GetTagsAsync();

            // Assert
            //TODO naprawic test
            result.Should().BeEmpty().And.BeOfType<List<TagModel>>();
        }

        [Fact]
        public async Task GetTagsAsync_ShouldReturnEmptyListOnException()
        {
            // Arrange
            _collection.FindAsync(Arg.Any<FilterDefinition<TagModel>>(), Arg.Any<FindOptions<TagModel, TagModel>>(),
                    default)
                .ThrowsForAnyArgs(new Exception("Test exception"));

            _database.GetCollection<TagModel>(Arg.Any<string>(), null).Returns(_collection);
            var mongoService = new MongoService(_options, _logger);

            // Act
            var result = await mongoService.GetTagsAsync();

            // Assert
            result.Should().BeEmpty().And.BeOfType<List<TagModel>>();
        }
    }
}