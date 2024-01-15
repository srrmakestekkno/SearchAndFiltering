using System.Collections.Concurrent;
using Dapper;
using webapi.Interfaces;
using webapi.Model;

namespace webapi.DB
{
    public class DsakDbRepo : IDsakDbRepo
    {
        private readonly ISqlHelper _sqlHelper;

        public DsakDbRepo(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public async Task<SearchResult> FindDsaksMatchingSearchStrings(Search search)
        {
            var idsOfPotentialDsaks = await GetIdsOfPotentialDsaks();
            var dsakIdsWithOccurences = CountOccurences();
            var dsakIds = GetDsaksToFetch();
            if (dsakIds.Length == 0)
            {
                return new SearchResult(Array.Empty<Dsak>(), search);
            }

            var numberOfTicketsFound = dsakIds.Length;
            if (dsakIds.Length > SearchResult.MaxItems)
            {
                dsakIds = dsakIds.Take(SearchResult.MaxItems).OrderByDescending(id => id).ToArray();
            }

            return await FetchDsaks();

            async Task<ConcurrentBag<IEnumerable<DsakId>>> GetIdsOfPotentialDsaks()
            {
                var potentialDsakIds = new ConcurrentBag<IEnumerable<DsakId>>();
                var getCandidateTasks = new Task[search.SearchTerms.Count];
                for (int i = 0; i < search.SearchTerms.Count; i++)
                {
                    getCandidateTasks[i] = GetCandidates(search.SearchTerms[i]);
                }

                await Task.WhenAll(getCandidateTasks);
                return potentialDsakIds;

                async Task GetCandidates(string searchTerm)
                {
                    const string Query = @"SELECT distinct ticket_id as Id FROM Mirror_Cust25129.dbo.ej_message WHERE (search_title like CONCAT('%',@term,'%') or body like CONCAT('%',@term,'%'))";
                    using (var connection = _sqlHelper.CreateConnection())
                    {
                        var dsakBatch = await connection.QueryAsync<DsakId>(Query, new { term = searchTerm }, commandTimeout: 120);
                        potentialDsakIds.Add(dsakBatch);
                    }
                }
            }

            Dictionary<DsakId, int> CountOccurences()
            {
                var dsakCounts = new Dictionary<DsakId, int>();
                foreach (var dsakBatch in idsOfPotentialDsaks)
                {
                    foreach (var dsakId in dsakBatch)
                    {
                        if (dsakCounts.ContainsKey(dsakId))
                        {
                            dsakCounts[dsakId]++;
                        }
                        else
                        {
                            dsakCounts.Add(dsakId, 1);
                        }
                    }
                }

                return dsakCounts;
            }

            int[] GetDsaksToFetch()
            {
                var ids = dsakIdsWithOccurences
                    .Where(d => d.Value == search.SearchTerms.Count)
                    .Select(d => d.Key.Id)
                    .ToArray();
                return ids;
            }

            async Task<SearchResult> FetchDsaks()
            {
                const string Sql = "select * from soanalyze.dbo.dsaksearch_v v where v.id in @ids order by v.id desc";
                using (var connection = _sqlHelper.CreateConnection())
                {
                    var tickets = (await connection.QueryAsync<Dsak>(Sql, new { ids = dsakIds })).ToArray();
                    if (numberOfTicketsFound > tickets.Length)
                    {
                        return new SearchResult(tickets, search, numberOfTicketsFound);
                    }

                    return new SearchResult(tickets, search);
                }
            }
        }

        public async Task<IEnumerable<CompanyDb>> GetUniqueCompanies()
        {
            var sql = @"select 
FirmaId as Id, 
FIRMANAVN as CompanyName, 
FIRMATYPE as CompanyType, 
FORVALTER as Manager
from soanalyze.dbo.Firma f
where f.FirmaType = 'Forvalter' and Id in 
order by f.FirmaNavn";

            
            using var conn = _sqlHelper.CreateConnection();
            var output = await conn.QueryAsync<CompanyDb>(sql);
            return output;
        }

        public async Task<IEnumerable<ProductDb>> GetUniqueProducts()
        {
            var sql = @"
select 
y.id as Id,
y.x_navn as ProductName,
y.x_active as Active
from Mirror_Cust25129.dbo.y_grunn y
";

            using var conn = _sqlHelper.CreateConnection();
            var output = await conn.QueryAsync<ProductDb>(sql);
            return output;
        }

        
    }
}
