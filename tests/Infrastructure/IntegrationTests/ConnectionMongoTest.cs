using Xunit;
using ProfileService.Core;
using System.Threading.Tasks;
using FluentAssertions;

namespace IntegrationTests;

[Collection("MongoDb")]
public class ConnectionMongoTest
{
    MongoDbTestBase _dbFixture;
    public ConnectionMongoTest(MongoDbTestBase dbFixture)
    {
        _dbFixture = dbFixture;
    }

    [Fact]
    public async Task CreateProfileTest()
    {
        var firstCount = await _dbFixture.Repo.CountDocumentsAsync(_dbFixture.allFilter);
        var user = new User("lgoin", "email", "password228hash");
        var profile = new Profile(user);


        await _dbFixture.Repo.InsertOneAsync(profile);


        var count = await _dbFixture.Repo.CountDocumentsAsync(_dbFixture.allFilter);
        count.Should().BeGreaterThan(firstCount);
    }

}
