using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SOApiProject.Data;
using SOApiProject.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute.ExceptionExtensions;


namespace SOApiTests.Unit

{
    public class DatabaseInitializerTests
    {
        private readonly ILogger<IDatabaseInitializer> _logger;
        private readonly IDatabaseInitializer _databaseInitializer;
        private readonly IApiService _apiService;

        public DatabaseInitializerTests()
        {
            _logger = Substitute.For<ILogger<IDatabaseInitializer>>();
            _apiService = new ApiService(new HttpClient());
            _databaseInitializer = new DatabaseInitializer( _logger,_apiService);
        }

        [Fact]
        public async Task FetchTagsFromApi_ShouldThrowException_IfAPIRequest_Fails()
        {
            // Act 
            Func<Task> act = async () => await _databaseInitializer.FetchTagsFromApi();

            //Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
        
    }
}