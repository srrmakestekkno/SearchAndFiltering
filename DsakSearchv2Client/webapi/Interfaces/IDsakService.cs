using webapi.DTOs;
using webapi.Model;

namespace webapi.Interfaces
{
    public interface IDsakService
    {
        Task<SearchResult> FindDsaksMatchingSearchStrings(Search search);
        Task<IEnumerable<Product>> GetAllProducts();
        Task<IEnumerable<Company>> GetAllCompanies();
    }
}
