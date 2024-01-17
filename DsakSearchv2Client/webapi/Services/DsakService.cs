using Microsoft.AspNetCore.Authentication;
using System.Linq;
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
            //static List<Product> GetUniqueProducts(Dsak[] tickets)
            //{
            //    var uniquProducts = new List<Product>();
            //    foreach (var ticket in tickets)
            //    {
            //        if (!uniquProducts.Any(x => x.ProductName == ticket.Product))
            //        {
            //            uniquProducts.Add(new Product(ticket.Id, ticket.Product, ticket.Status));
            //        }
            //    }

            //    return uniquProducts;
            //}

            //static List<Company> GetUniqueCompanies(Dsak[] tickets)
            //{
            //    var uniqueCompnanies = new List<Company>();
            //    foreach (var ticket in tickets)
            //    {
            //        if (!uniqueCompnanies.Any(x => x.Manager == ticket.Manager))
            //        {
            //            uniqueCompnanies.Add(new Company(ticket.Id, string.Empty, string.Empty, ticket.Manager));
            //        }

            //    }
            //    return uniqueCompnanies;
            //}
            
            //static List<DipsVersion> GetUniqueDipsVersions(Dsak[] tickets)
            //{
            //    var uniqueVersion = new List<DipsVersion>();

            //    foreach (var ticket in tickets)
            //    {
            //        if (!uniqueVersion.Any(x => x.Name == ticket.Version))
            //        {
            //            uniqueVersion.Add(new DipsVersion(ticket.Version_Id, ticket.Version));
            //        }
            //    }

            //    return uniqueVersion;
            //}

            
            

            var searchResult = await _dbRepo.FindDsaksMatchingSearchStrings(search);

            var uniqueManagers= new List<Company>(); // Endre til å bruker manager
            var uniquProducts = new List<Product>();            
            var uniqueVersions = new List<DipsVersion>();
            
            foreach (var ticket in searchResult.Tickets)
            {
                if (!uniqueManagers.Any(x => x.Manager == ticket.Manager))
                {
                    uniqueManagers.Add(new Company(ticket.Id, string.Empty, string.Empty, ticket.Manager));
                }

                if (!uniquProducts.Any(x => x.ProductName == ticket.Product))
                {
                    uniquProducts.Add(new Product(ticket.Id, ticket.Product, ticket.Status));
                }

                if (!uniqueVersions.Any(x => x.Name == ticket.Version))
                {
                    uniqueVersions.Add(new DipsVersion( ticket.Version_Id, ticket.Version));
                }                
            }

            //var uniqueCompnanies = GetUniqueCompanies(searchResult.Tickets);
            //var uniquProducts = GetUniqueProducts(searchResult.Tickets);
            //var uniqueDipsVersion = GetUniqueDipsVersions(searchResult.Tickets);
            
            searchResult.Companies = uniqueManagers;
            //searchResult.NumberOfManagers = uniqueCompnanies.Count; // brukes ikke
            searchResult.Products = uniquProducts;  
            searchResult.Versions = uniqueVersions;

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
