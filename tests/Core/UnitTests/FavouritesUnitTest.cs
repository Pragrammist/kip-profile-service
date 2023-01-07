using Xunit;
using ProfileService.Core;
using FluentAssertions;
using System.Linq;

namespace UnitTests;

public class FavouriteUnitTest
{
    [Fact]
    public void Add()
    {
        WillWatch w = new WillWatch();

        w.Add("qwe");

        w.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void AddException()
    {
        WillWatch w = new WillWatch();
        w.Add("qwe");


        Assert.Throws<FavouriteFilmAlreadyExistsException>(() =>
        {
            w.Add("qwe");
        });


    }


    [Fact]
    public void Remove()
    {
        WillWatch w = new WillWatch();

        w.Add("qwe");
        w.Remove("qwe");

        w.Count.Should().Be(0);
    }

    [Fact]
    public void RemoveAt()
    {
        WillWatch w = new WillWatch();

        w.Add("qwe");
        w.RemoveAt(0);

        w.Count.Should().Be(0);
    }


    [Fact]
    public void Insert()
    {
        WillWatch w = new WillWatch();

        w.Add("qwe");
        w[0] = "qwe2";

        w.Contains("qwe2").Should().Be(true);
    }

    [Fact]
    public void InsertOutrangeException()
    {
        WillWatch w = new WillWatch();

        w.Add("qwe");

        Assert.Throws<IndexOutOfRangeFavouriteException>(() =>
        {
            w[1] = "qwe2";
        });
    }
    [Fact]
    public void InsertAlreadyExistsException()
    {
        WillWatch w = new WillWatch();

        w.Add("qwe");

        Assert.Throws<FavouriteFilmAlreadyExistsException>(() =>
        {
            w[1] = "qwe";
        });
    }

    [Theory]
    [InlineData("qwe", 0)]
    [InlineData("qwe2", -1)]
    public void IndexOf(string value, int expectedIndex)
    {
        WillWatch w = new WillWatch();

        w.Add("qwe");

        w.IndexOf(value).Should().Be(expectedIndex);
    }

    [Fact]
    public void EnumarableTest()
    {
        WillWatch w = new WillWatch();

        w.Add("qwe");

        w.First();
        Assert.True(true);
    }
}
