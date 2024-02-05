using webapi.DB;
using webapi.Model;

namespace webapi.Interfaces
{
    public interface IDsakDbRepo
    {
        Task<SearchResult> FindDsaksMatchingSearchStrings(Search search, bool includeFront);
    }
}
