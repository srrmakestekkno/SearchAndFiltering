using Moq;
using webapi.Helpers;
using webapi.Interfaces;
using webapi.Model;
using webapi.Services;

namespace DsakSearchTests
{
    public class DsakServiceTests
    {
        private IDsakService _dsakService;
        private Mock<IDsakDbRepo> _dbRepoMock;

        [SetUp]
        public void Setup()
        {
            _dbRepoMock = new Mock<IDsakDbRepo>();
            _dsakService = new DsakService(_dbRepoMock.Object);
        }

        [TestCase("screenit", false)]
        public async Task FindDsaksMatchingSearchStrings_ReturnsData(string searcTerm, bool includeFront)
        {
            

            var searchResult = new SearchResult();
            _dbRepoMock.Setup(x => x.FindDsaksMatchingSearchStrings(SearchParser.ParseSearchString(searcTerm), includeFront))
                .ReturnsAsync(searchResult);
            Assert.Pass();
        }

        
    }
}