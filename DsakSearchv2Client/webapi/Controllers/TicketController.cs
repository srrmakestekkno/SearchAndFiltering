using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using webapi.Helpers;
using webapi.Interfaces;
using webapi.Model;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class TicketController : ControllerBase
{
    private readonly IDsakService _dsakService;
    readonly IMemoryCache _memoryCache;

    private readonly MemoryCacheEntryOptions _cacheEntryOptions;
    private readonly ILogger<TicketController> _logger;

    public TicketController(ILogger<TicketController> logger, IDsakService dsakService, IMemoryCache memoryCache)
    {    
        _logger = logger;
        _dsakService = dsakService;
        _memoryCache = memoryCache;
        _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1D))
            .SetAbsoluteExpiration(TimeSpan.FromHours(6D));
    }

    [HttpPost("search")]
    public async Task<ActionResult<SearchResult>> Search([FromHeader] string callId, [FromHeader] bool includeFront, [FromBody] string searchString)
    {
        async Task<SearchResult> Search(Search search, bool includeFront)
        {
            var key = search.SearchString + "_" + includeFront;
            if (!_memoryCache.TryGetValue(key, out SearchResult searchResult))
            {
                searchResult = await _dsakService.FindDsaksMatchingSearchStrings(search, includeFront);
                _memoryCache.Set(key, searchResult, _cacheEntryOptions);
            }

            return searchResult;
        }

        try
        {
            Request.HttpContext.Response.Headers.Add("callId", callId);
            var search = SearchParser.ParseSearchString(searchString);
            if (!search.IsValid)
            {
                return new SearchResult("Søkestrengen inneholdt ingen gyldige bokstaver");
            }

            return await Search(search, includeFront);
        }
        catch (Exception e)
        {
            return new SearchResult($"{e.Message}");
        }
        
    }
}