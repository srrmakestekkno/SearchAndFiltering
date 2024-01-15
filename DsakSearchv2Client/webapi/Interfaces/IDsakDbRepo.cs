using webapi.DB;
using webapi.Model;

namespace webapi.Interfaces
{
    public interface IDsakDbRepo
    {
        Task<SearchResult> FindDsaksMatchingSearchStrings(Search search);
        Task<IEnumerable<CompanyDb>> GetUniqueCompanies();
        Task<IEnumerable<ProductDb>> GetUniqueProducts();
    }
}
