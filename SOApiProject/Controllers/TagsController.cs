using Microsoft.AspNetCore.Mvc;
using SOApiProject.Data;
using SOApiProject.Models;

namespace SOApiProject.Controllers;

[ApiController]
[Route("api/so")]
public class TagsController : ControllerBase
{
    private readonly MongoService _mongoService;
    private readonly DatabaseInitializer _databaseInitializer;

    public TagsController(MongoService mongoService, DatabaseInitializer databaseInitializer)
    {
        _mongoService = mongoService;
        _databaseInitializer = databaseInitializer;
    }

    /// <summary>
    /// udział tagów w populacji
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<Dictionary<string, double>> GetTagsShare()
    {
        var result = new Dictionary<string, double>();
    
        var tags = _mongoService.GetTagsAsync().Result;
        long totalTags = 0;
        foreach (var tag in tags)
        {
            totalTags += tag.Count;
        }
    
        foreach (var tag in tags)
        {
            //TODO zlikwidowac  chyba nie bedziemy miec duplikatow
            if (result.ContainsKey(tag.Name))
            {
                continue;
            }
            double share = (double)tag.Count * 100 / totalTags;
            result.Add(tag.Name,share);
        }
    
        return result;
    }
    [HttpGet]
    public async Task<List<PagedTagModel>> GetTags(string sortBy = "name", bool ascending = true, int pageSize = 10)
    {
        var tagModels = _mongoService.GetTagsAsync().Result;
        var result = new List<PagedTagModel>();
        switch (sortBy.ToLower())
        {
            case "name":
                tagModels = new List<TagModel>(ascending
                    ? tagModels.OrderBy(t => t.Name)
                    : tagModels.OrderByDescending(t => t.Name));
                break;
            case "count":
                tagModels = new List<TagModel>(ascending
                    ? tagModels.OrderBy(t => t.Count)
                    : tagModels.OrderByDescending(t => t.Count));
                break;
        }

        var pageCount = (int)Math.Ceiling((double)tagModels.Count / pageSize);

        for (int i = 0; i < pageCount; i++)
        {
            var page = tagModels.Skip(i *pageSize). Take(pageSize).ToList();
            result.Add(new PagedTagModel
            {
                Content = page,
                PageNumber = i + 1
            });
        }

        return result;
    }

    [HttpPost]
    public async Task RepopulateDatabase(int tagsAmount = 1000)
    {
        if (tagsAmount < 1000)
            throw new ArgumentException("Tags amount cannot be less than 1000");
        
        await _mongoService.DropCollection();
        await _databaseInitializer.InitDb();

    }
}