using Xunit;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests;

[CollectionDefinition("WebContext")]
public class WebFixtireCollection : ICollectionFixture<WebFixture>
{
    
}
