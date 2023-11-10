using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SECrawler.Business.Services;
using SECrawler.Dtos;


namespace SECrawler.Tests.Services
{
    [TestFixture]
    public class SearchResultServiceTests
    {
        [Test]
        public async Task GetAllHistoriesAsync_ShouldReturnMappedResults()
        {
            // Arrange
            var mockDataService = new Mock<ISearchResultRepository>();
            var service = new SearchResultService(mockDataService.Object);

            
            var repositoryData = new List<Entities.SearchResult>
            {
                new Entities.SearchResult { Id = 1, Date = DateTime.Now, Rank = "1", Url = "http://example.com", KeyWords = "test" },
                
            };

            mockDataService.Setup(repo => repo.GetHistoriesAsync()).ReturnsAsync(repositoryData);

            // Act
            var result = await service.GetAllHistoriesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result, Is.InstanceOf<IEnumerable<SearchResult>>());

            var resultList = result.ToList();
            Assert.That(resultList.Count, Is.EqualTo(repositoryData.Count));
            
            Assert.That(resultList[0].Id, Is.EqualTo(repositoryData[0].Id));
            Assert.That(resultList[0].Date, Is.EqualTo(repositoryData[0].Date));
            Assert.That(resultList[0].Rank, Is.EqualTo(repositoryData[0].Rank));
            Assert.That(resultList[0].Url, Is.EqualTo(repositoryData[0].Url));
            Assert.That(resultList[0].KeyWords, Is.EqualTo(repositoryData[0].KeyWords));
        }
    }
}