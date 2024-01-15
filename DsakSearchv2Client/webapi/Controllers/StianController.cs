using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using webapi.DTOs;
using webapi.Helpers;
using webapi.Interfaces;
using webapi.Model;
using webapi.Services;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class StianController : ControllerBase
{
    private readonly IDsakService _dsakService;
    readonly IMemoryCache _memoryCache;

    private readonly MemoryCacheEntryOptions _cacheEntryOptions;
    private readonly ILogger<StianController> _logger;

    public StianController(ILogger<StianController> logger, IDsakService dsakService, IMemoryCache memoryCache)
    {    
        _logger = logger;
        _dsakService = dsakService;
        _memoryCache = memoryCache;
        _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1D))
            .SetAbsoluteExpiration(TimeSpan.FromHours(6D));
    }

    [HttpPost("search")]
    public async Task<ActionResult<SearchResult>> Search([FromHeader] string callId, [FromBody] string searchString)
    {
        async Task<SearchResult> Search(Search search)
        {
            var key = search.SearchString;
            if (!_memoryCache.TryGetValue(key, out SearchResult searchResult))
            {
                searchResult = await _dsakService.FindDsaksMatchingSearchStrings(search);
                _memoryCache.Set(key, searchResult, _cacheEntryOptions);
            }

            return searchResult;
        }


        Request.HttpContext.Response.Headers.Add("callId", callId);
        var search = SearchParser.ParseSearchString(searchString);
        if (!search.IsValid)
        {
            return new SearchResult("Søkestrengen inneholdt ingen gyldige bokstaver");
        }

        return await Search(search);
    }

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return Ok(await _dsakService.GetAllProducts());
    }

    [HttpGet("companies")]
    public async Task<ActionResult<Dictionary<Company, int>>> GetCompanies()
    {
        return Ok(await _dsakService.GetAllCompanies());
    }
}