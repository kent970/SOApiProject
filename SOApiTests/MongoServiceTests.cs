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
        private readonly IDatabaseInitializer _databaseInitializer;
        private readonly IMongoService _mongoService;

        public MongoServiceTests()
        {
            _logger = Substitute.For<ILogger<MongoService>>();
            _options = Substitute.For<IOptions<MongoDBSettings>>();
            _collection = Substitute.For<IMongoCollection<TagModel>>();
            _database = Substitute.For<IMongoDatabase>();
            _databaseInitializer = Substitute.For<IDatabaseInitializer>();

            _options.Value.Returns(new MongoDBSettings
            {
                ConnectionURI = "mongodb://localhost:27017",
                DatabaseName = "testdb",
                CollectionName = "testcollection"
            });
            _mongoService = new MongoService(_options, _logger, _databaseInitializer);
        }

        [Fact]
        public async Task GetTagsAsync_ShouldReturnEmptyListOnException()
        {
            //Arrange
            _collection.FindAsync(Arg.Any<FilterDefinition<TagModel>>(), Arg.Any<FindOptions<TagModel, TagModel>>(),
                    default)
                .ThrowsForAnyArgs(new Exception());

            _database.GetCollection<TagModel>(Arg.Any<string>()).Returns(_collection);

            // Act
            var result = await _mongoService.GetSortedTags("Name", true, 90);

            // Assert
            result.Should().BeEmpty().And.BeOfType<List<PagedTagModel>>();
        }

        [Fact]
        public async Task DropCollection_ShouldLogInformationOnSucces()
        {
            //Act
            _mongoService.DropCollection();

            //Assert
            _logger.Received(1).LogInformation($"Successfully dropped {_options.Value.CollectionName} collection");
        }
    }
}