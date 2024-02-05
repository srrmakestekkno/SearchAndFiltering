using webapi.Model;

namespace webapi.Interfaces
{
    public interface IDsakService
    {
        Task<SearchResult> FindDsaksMatchingSearchStrings(Search search, bool includeFront);
    }
}
