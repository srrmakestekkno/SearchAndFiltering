using webapi.DTOs;
using webapi.Interfaces;
using webapi.Model;

namespace webapi.Services
{
    public class DsakService : IDsakService
    {
        private readonly IDsakDbRepo _dbRepo;

        public DsakService(IDsakDbRepo dsakDbRepo)
        {
            _dbRepo = dsakDbRepo;
        }

        public async Task<SearchResult> FindDsaksMatchingSearchStrings(Search search, bool includeFront)
        {
            var searchResult = await _dbRepo.FindDsaksMatchingSearchStrings(search, includeFront);

            var uniqueManagers = new List<Manager>();
            var uniqueCompanies = new List<Company>();
            var uniquProducts = new List<Product>();
            var uniqueVersions = new List<DTOs.Version>();

            foreach (var ticket in searchResult.Tickets)
            {
                if (!uniqueManagers.Any(x => x.ManagerName == ticket.Manager))
                {
                    uniqueManagers.Add(new Manager(ticket.Id, ticket.Manager));
                }

                if (!uniqueCompanies.Any(x => x.CompanyName == ticket.Company))
                {
                    uniqueCompanies.Add(new Company(ticket.Id, ticket.Company));
                }

                if (!uniquProducts.Any(x => x.ProductName == ticket.Product))
                {
                    uniquProducts.Add(new Product(ticket.Id, ticket.Product, ticket.Status));
                }

                if (!uniqueVersions.Any(x => x.Name == ticket.Version))
                {
                    uniqueVersions.Add(new DTOs.Version(ticket.Version_Id, ticket.Version));
                }
            }

            searchResult.Managers = uniqueManagers;
            searchResult.Companies = uniqueCompanies;
            searchResult.Products = uniquProducts;
            searchResult.Versions = uniqueVersions;          

            return searchResult;
        }
    }
}
