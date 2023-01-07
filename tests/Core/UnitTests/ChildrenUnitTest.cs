using System.Collections.Generic;
using Xunit;
using ProfileService.Core;
using FluentAssertions;
using System.Linq;

namespace UnitTests;

public class ChildrenUnitTest
{

    static ChildProfile child => new ChildProfile(0, "jack");
    static ChildProfile secondChild => new ChildProfile(0, "tom");
    [Fact]
    public void Add()
    {
        Childern children = new Childern();

        children.Add(child);

        children.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void AddException()
    {
        Childern children = new Childern();
        children.Add(child);


        Assert.Throws<ChildAlreadyExistsException>(() =>
        {
            children.Add(child);
        });


    }


    [Fact]
    public void Remove()
    {
        Childern children = new Childern();

        children.Add(child);
        children.Remove(child);

        children.Count.Should().Be(0);
    }

    [Fact]
    public void RemoveAt()
    {
        Childern children = new Childern();

        children.Add(child);
        children.RemoveAt(0);

        children.Count.Should().Be(0);
    }


    [Fact]
    public void Insert()
    {
        Childern children = new Childern();

        children.Add(child);
        children[0] = secondChild;

        children.Contains(secondChild).Should().Be(true);
    }

    [Fact]
    public void InsertOutrangeException()
    {
        Childern children = new Childern();

        children.Add(child);

        Assert.Throws<ChildIndexOutOfRangeException>(() =>
        {
            children[1] = secondChild;
        });
    }
    [Fact]
    public void InsertAlreadyExistsException()
    {
        Childern children = new Childern();

        children.Add(child);

        Assert.Throws<ChildAlreadyExistsException>(() =>
        {
            children[0] = child;
        });
    }

    [Theory]
    [MemberData(nameof(IndexOfInline))]
    public void IndexOf(ChildProfile value, int expectedIndex)
    {
        Childern children = new Childern();

        children.Add(child);

        children.IndexOf(value).Should().Be(expectedIndex);
    }

    [Fact]
    public void EnumarableTest()
    {
        Childern children = new Childern();


        children.Add(child);

        children.First();
        Assert.True(true);
    }

    static IEnumerable<object[]> IndexOfInline()
    {

        yield return new object[] { child, 0 };
        yield return new object[] { secondChild, -1 };
    }
}