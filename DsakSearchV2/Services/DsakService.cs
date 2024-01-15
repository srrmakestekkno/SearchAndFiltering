using DsakSearchV2.Model;
using Dapper;
using System.Data.SqlClient;
using System.Collections.Concurrent;
using DsakSearchV2.Services.Db;
using DsakSearchV2.Configuration;
using DsakSearchV2.DTOs;

namespace DsakSearchV2.Services
{
    public class DsakService : IDsakService
    {      
        private readonly ISqlHelper _sqlHelper;
        private readonly IDsakConfiguration _dsakConfiguration;

        public DsakService(ISqlHelper sqlHelper, IDsakConfiguration dsakConfiguration)
        {
            _sqlHelper = sqlHelper;
            _dsakConfiguration = dsakConfiguration;
        }

        public async Task<IEnumerable<DsakDto>> FindDsaksMatchingSearchStrings(Search search)
        {
            var sql = "select * from Mirror_Cust25129.dbo.category";
            var connectionString = _dsakConfiguration.GetConnectionString();//$"Data Source={datasource}; User ID={username};Password={password}";
            try
            {
                using var connection = new SqlConnection(connectionString);
                var output = await connection.QueryAsync<DummyRecord>(sql);

                var result = DbMapper.MapToDsakDto(output);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return new List<DsakDto>();

            //ConcurrentBag<IEnumerable<DsakId>> idsOfPotentialDsaks = await GetIdsOfPotentialDsaks(search);
            //Dictionary<DsakId, int> dsakIdsWithOccurences = CountOccurences(idsOfPotentialDsaks);
            //var dsakIds = GetDsaksToFetch(dsakIdsWithOccurences, search);
            //if (dsakIds.Length == 0)
            //{
            //    return new SearchResult(Array.Empty<Dsak>(), search);
            //}

            //var numberOfTicketsFound = dsakIds.Length;
            //if (dsakIds.Length > SearchResult.MaxItems)
            //{
            //    dsakIds = dsakIds.Take(SearchResult.MaxItems).OrderByDescending(id => id).ToArray();
            //}

            //return await FetchDsaks(search, numberOfTicketsFound, dsakIds);
        }


        private async Task<SearchResult> FetchDsaks(Search search, int numberOfTicketsFound, int[]? dsakIds)
        {
            const string Sql = "select id as id, title, created_at as createdAt, x_company_name as customer, X_OVERFORTVSTS as tfs from crm7.ticket where id in @ids order by id desc";
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

        private static int[] GetDsaksToFetch(Dictionary<DsakId, int> dsakIdsWithOccurences, Search search)
        {
            var ids = dsakIdsWithOccurences
                   .Where(d => d.Value == search.SearchTerms.Count)
                   .Select(d => d.Key.Id)
                   .ToArray();
            return ids;
        }

        private static Dictionary<DsakId, int> CountOccurences(ConcurrentBag<IEnumerable<DsakId>> idsOfPotentialDsaks)
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

        private async Task<ConcurrentBag<IEnumerable<DsakId>>> GetIdsOfPotentialDsaks(Search search)
        {
            var potentialDsakIds = new ConcurrentBag<IEnumerable<DsakId>>();
            var getCandidateTasks = new Task[search.SearchTerms.Count];
            for (int i = 0; i < search.SearchTerms.Count; i++)
            {
                getCandidateTasks[i] = GetCandidates(search.SearchTerms[i], potentialDsakIds);
            }

            await Task.WhenAll(getCandidateTasks);
            return potentialDsakIds;
            
        }

        private async Task GetCandidates(string searchTerm, ConcurrentBag<IEnumerable<DsakId>> potentialDsakIds)
        {
            const string Query = "SELECT distinct ticket_id as Id FROM crm7.ej_message WHERE (search_title like CONCAT('%',@term,'%') or body like CONCAT('%',@term,'%'))";
            using (var connection = _sqlHelper.CreateConnection())
            {
                var dsakBatch = await connection.QueryAsync<DsakId>(Query, new { term = searchTerm }, commandTimeout: 120);
                potentialDsakIds.Add(dsakBatch);
            }
        }
    }
}
