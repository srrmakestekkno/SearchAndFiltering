using DsakSearchV2.Services;
using Microsoft.AspNetCore.Mvc;

namespace DsakSearchV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DsakSearchController : ControllerBase
    {
        private readonly IDsakService _dsakService;
        //readonly IMemoryCache _memoryCache;

        //private readonly MemoryCacheEntryOptions cacheEntryOptions;

        public DsakSearchController(IDsakService dsakService/*, IMemoryCache memoryCache*/)
        {
            _dsakService = dsakService;
            //_memoryCache = memoryCache;

            //cacheEntryOptions = new MemoryCacheEntryOptions()
            //    .SetSlidingExpiration(TimeSpan.FromHours(1D))
            //    .SetAbsoluteExpiration(TimeSpan.FromHours(6D));
        }

        [HttpGet("search")]
        public async Task<IEnumerable<DsakDto>> Get(/*string callId, */string searchString)
        {
            //Request.HttpContext.Response.Headers.Add("callId", callId);
            
            var search = SearchParser.ParseSearchString(searchString);
            if (!search.IsValid)
            {
                //return new SearchResult("Søkestrengen inneholdt ingen gyldige bokstaver");
            }

            return await Search(search);
        }

        private async Task<IEnumerable<DsakDto>> Search(Search search)
        {
            var key = search.SearchString;
            //if (!_memoryCache.TryGetValue(key, out SearchResult searchResult))
            //{
                
            //    _memoryCache.Set(key, searchResult, cacheEntryOptions);
            //}

            var searchResult = await _dsakService.FindDsaksMatchingSearchStrings(search);

            return searchResult;
        }
    }
}