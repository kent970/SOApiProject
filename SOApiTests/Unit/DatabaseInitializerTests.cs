using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SOApiProject.Data;
using SOApiProject.Models;
using FluentAssertions;
using NSubstitute.ExceptionExtensions;


namespace SOApiTests.Unit

{
    public class DatabaseInitializerTests
    {
        private readonly IMongoService _mongoService;
        private readonly ILogger<IDatabaseInitializer> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDatabaseInitializer _databaseInitializer;

        public DatabaseInitializerTests()
        {
            _mongoService = Substitute.For<IMongoService>();
            _logger = Substitute.For<ILogger<IDatabaseInitializer>>();
            _configuration = Substitute.For<IConfiguration>();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _databaseInitializer = new DatabaseInitializer(_httpClientFactory, _configuration, _mongoService, _logger);
        }

        [Fact]
        public async Task InitDb_Should_Return_Without_Adding_Tags_If_Collection_Size_Is_Greater_Than_Tags_Amount()
        {
            // Arrange
            _mongoService.GetCollectionSize().Returns(2000);

            // Act
            await _databaseInitializer.InitDb(tagsAmount: 1000);

            // Assert
            await _mongoService.DidNotReceive().AddToTagModelsAsync(Arg.Any<List<TagModel>>());
        }


        [Fact]
        public async Task InitDb_Should_Throw_Exception_If_API_Request_Fails()
        {
            // Arrange
            _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient());

            // Act 
            Func<Task> act = async () => await _databaseInitializer.InitDb();

            //Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}