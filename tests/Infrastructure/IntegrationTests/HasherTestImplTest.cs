using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure;

namespace IntegrationTests;

public class HasherTestImplTest
{
    [Fact]
    public async Task HasherTest()
    {
        var p = new PasswordHasherImpl();
        var str = "randomstr";
        var hash = await p.Hash(str);

        hash.Should().NotBe(str);
    }
}
