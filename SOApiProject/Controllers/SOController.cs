// using System.Text.Json;
// using Microsoft.AspNetCore.Mvc;
// using MongoDB.Driver;
// using MongoDB.Entities;
// using SOApiProject.Data;
// using SOApiProject.Models;
//
// namespace SOApiProject.Controllers;
//
// [ApiController]
// [Route("api/so")]
// public class SOController : ControllerBase
// {
//     private readonly IHttpClientFactory _httpClientFactory;
//     private readonly IConfiguration _configuration;
//     private readonly MongoService _mongoService;
//     
//
//     public SOController(IHttpClientFactory httpClientFactory, IConfiguration configuration, MongoService mongoService)
//     {
//         _httpClientFactory = httpClientFactory;
//         _configuration = configuration;
//         _mongoService = mongoService;
//     }
//
//     [HttpGet]
//     public async Task<List<TagModel>> GetTags(int startPage = 1, int pageSize = 100, int pageCount = 1)
//     {
//         var tagModels = new List<TagModel>();
//         var pageNum = startPage;
//
//         var httpClient = _httpClientFactory.CreateClient("SOClient");
//
//         string apiBaseUrl = _configuration["BaseUrl"];
//
//         for (int i = pageNum; i <= pageCount; i++)
//         {
//             string apiUrl =
//                 $"{apiBaseUrl}?page={startPage}&pagesize={pageSize}&order=desc&sort=popular&site=stackoverflow";
//
//             var response = await httpClient.GetAsync(apiUrl);
//
//             response.EnsureSuccessStatusCode();
//
//             if (response.IsSuccessStatusCode)
//             {
//                 var contentStream = await response.Content.ReadAsStreamAsync();
//                 using (var gzip = new System.IO.Compression.GZipStream(contentStream,
//                            System.IO.Compression.CompressionMode.Decompress))
//                 {
//                     var options = new JsonSerializerOptions
//                     {
//                         PropertyNameCaseInsensitive = true
//                     };
//                     var result = JsonSerializer.Deserialize<Response>(gzip, options);
//                     
//                     tagModels.AddRange(result.Items);
//                 }
//             }
//         }
//
//         await _mongoService.AddToTagModelsAsync(tagModels);
//         return tagModels;
//     }
// }