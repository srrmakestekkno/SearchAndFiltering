using DsakSearchV2.DTOs;
using DsakSearchV2.Model;
using DsakSearchV2.Services.Db;

namespace DsakSearchV2.Services
{
    public interface IDsakService
    {
        Task<IEnumerable<DsakDto>> FindDsaksMatchingSearchStrings(Search search);
    }
}
