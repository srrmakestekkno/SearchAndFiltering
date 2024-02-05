using webapi.DB;
using webapi.DTOs;

namespace webapi.Services
{
    public static class DbMapper
    {
        public static bool IsTrue(int value) => value == 0;

        public static Product MapToProduct(ProductDb productDb)
        {
            var product = new Product(productDb.Id, productDb.ProductName, productDb.Active);            

            return product;
        }

        public static Manager MapToCompany(CompanyDb company)
        {
            var dto = new Manager(company.Id, company.Manager);

            return dto;
        }
    }
}
