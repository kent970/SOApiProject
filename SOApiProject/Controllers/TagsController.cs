using Microsoft.AspNetCore.Mvc;
using SOApiProject.Data;
using SOApiProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOApiProject.Controllers
{
    [ApiController]
    [Route("api/so")]
    public class TagsController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        private readonly IDatabaseInitializer _databaseInitializer;

        public TagsController(IMongoService mongoService, IDatabaseInitializer databaseInitializer)
        {
            _mongoService = mongoService;
            _databaseInitializer = databaseInitializer;
        }

        /// <summary>
        /// Gets the share of tags.
        /// </summary>
        /// <returns>A dictionary containing the share of each tag.</returns>
        [HttpGet("tags/share")]
        [ProducesResponseType(200, Type = typeof(Dictionary<string, double>))]
        public async Task<ActionResult<Dictionary<string, double>>> GetTagsShare()
        {
            var result = new Dictionary<string, double>();

            var tags = await _mongoService.GetTagsAsync();
            long totalTags = tags.Sum(tag => tag.Count);

            foreach (var tag in tags)
            {
                double share = (double)tag.Count * 100 / totalTags;
                result.Add(tag.Name, share);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the tags with pagination and sorting.
        /// </summary>
        /// <param name="sortBy">The field to sort by Name or Count.</param>
        /// <param name="ascending">Specifies sort order.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <returns>A list of paged tag models.</returns>
        [HttpGet("tags")]
        [ProducesResponseType(200, Type = typeof(List<PagedTagModel>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<PagedTagModel>>> GetTags(string sortBy = "Name", bool ascending = true, int pageSize = 100)
        {
            if (!sortBy.Equals("Name") && !sortBy.Equals("Count"))
                return BadRequest("Sorting can only be done by Name or Count");

            var tagModels = await _mongoService.GetTagsAsync(sortBy, ascending);
            
            if (!tagModels.Any())
                return NotFound("No tags found");
            
            var result = new List<PagedTagModel>();

            var pageCount = (int)Math.Ceiling((double)tagModels.Count / pageSize);

            for (int i = 0; i < pageCount; i++)
            {
                var page = tagModels.Skip(i * pageSize).Take(pageSize).ToList();
                result.Add(new PagedTagModel
                {
                    Content = page,
                    PageNumber = i + 1
                });
            }

            return Ok(result);
        }

        /// <summary>
        /// Repopulates the database with tags.
        /// </summary>
        /// <param name="tagsAmount">The number of tags to generate.</param>
        [HttpPost("tags/repopulate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RepopulateDatabase(int tagsAmount = 1000)
        {
            if (tagsAmount < 1000)
                return BadRequest("Tags amount cannot be less than 1000");

            await _mongoService.DropCollection();
            await _databaseInitializer.InitDb();

            return Ok();
        }
    }
}
