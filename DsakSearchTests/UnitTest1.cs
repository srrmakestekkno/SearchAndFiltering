using Moq;
using webapi.Interfaces;
using webapi.Services;

namespace DsakSearchTests
{
    public class DsakServiceTests
    {
        private readonly IDsakService _dsakService;
        private readonly Mock<IDsakDbRepo> _dbRepoMock;

        [SetUp]
        public void Setup()
        {
            _dsakService = new DsakService();
        }

        [TestCase("screenit")]
        public async Task FindDsaksMatchingSearchStrings_ReturnsData(string searcT)
        {

            Assert.Pass();
        }
    }
}