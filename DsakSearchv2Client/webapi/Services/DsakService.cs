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

        public async Task<SearchResult> FindDsaksMatchingSearchStrings(Search search)
        {
            static List<Product> GetUniqueProducts(Dsak[] tickets)
            {
                var uniquProducts = new List<Product>();
                foreach (var ticket in tickets)
                {
                    if (!uniquProducts.Any(x => x.ProductName == ticket.Product))
                    {
                        uniquProducts.Add(new Product(ticket.Id, ticket.Product, ticket.Status));
                    }
                }

                return uniquProducts;
            }

            static List<Company> GetUniqueCompanies(Dsak[] tickets)
            {
                var uniqueCompnanies = new List<Company>();
                foreach (var ticket in tickets)
                {
                    if (!uniqueCompnanies.Any(x => x.Manager == ticket.Manager))
                    {
                        uniqueCompnanies.Add(new Company(ticket.Id, string.Empty, string.Empty, ticket.Manager));
                    }

                }
                return uniqueCompnanies;
            }
            
            

            var searchResult = await _dbRepo.FindDsaksMatchingSearchStrings(search);

            // Finner unike forvaltere og antall treff på disse i søket
            //var companyOccurances = new Dictionary<string, int>();
            //foreach (var item in searchResult.Tickets)
            //{
            //    if (!companyOccurances.ContainsKey(item.Company))
            //    {
            //        companyOccurances.Add(item.Company, 1);
            //    }
            //    else
            //    {
            //        companyOccurances[item.Company] += 1;
            //    }
            //}
            //searchResult.UniqueCompanyOccurances = companyOccurances;
            // End

            var uniqueCompnanies = GetUniqueCompanies(searchResult.Tickets);
            var uniquProducts = GetUniqueProducts(searchResult.Tickets);
            
            searchResult.Companies = uniqueCompnanies;
            searchResult.NumberOfManagers = uniqueCompnanies.Count;
            searchResult.Products = uniquProducts;            

            return searchResult;
        }

        public async Task<IEnumerable<Company>> GetAllCompanies()
        {
            var dtos = new List<Company>();
            var companiesFromDb = await _dbRepo.GetUniqueCompanies();

            foreach (var company in companiesFromDb)
            {
                dtos.Add(DbMapper.MapToCompany(company));
            }

            return dtos;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = await _dbRepo.GetUniqueProducts();

            var dtos = new List<Product>();

            foreach (var product in products)
            {
                dtos.Add(DbMapper.MapToProduct(product));
            }

            return dtos;            
        }
    }
}
