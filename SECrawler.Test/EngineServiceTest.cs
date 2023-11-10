using System.Reflection;
using Moq.Protected;

namespace SECrawler.Tests.Services;

using Moq;
using SECrawler.Business.Services;
using SECrawler.Entities;

[TestFixture]
public class Tests
{
    

    [Test]
    public async Task GetRankingsAsync_WithInvalidEngineId_ReturnsEmptyList()
    {
        // Arrange
        var dataServiceMock = new Mock<IEngineRepository>();
        var resultDataServiceMock = new Mock<ISearchResultRepository>();

        var googleEngineService = new GoogleEngineService(dataServiceMock.Object, resultDataServiceMock.Object);

        var engineId = 1;
        var pageSize = 10;
        var query = "test query";

        dataServiceMock.Setup(x => x.GetOneAsync(engineId)).ReturnsAsync((Engine)null);

        // Act
        var result = await googleEngineService.GetRankingsAsync(query, engineId, pageSize);

        // Assert
        Assert.IsInstanceOf<List<int>>(result);
        Assert.AreEqual(0, result.Count);
    }
  
}
