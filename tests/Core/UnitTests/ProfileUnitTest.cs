using Xunit;
using ProfileService.Core;
using FluentAssertions;
using System.Collections.Generic;

namespace UnitTests;
public class ProfileUnitTest
{


    [Theory]
    [MemberData(nameof(ProfileFavouriteData))]
    public void TestFavourites(FavouriteBase favorite)
    {
        favorite.Add("filmId1");

        favorite.Count.Should().Be(1);
    }

    [Fact]
    public void TestChilds()
    {
        Profile profile = new Profile("ID", new User("LOGIN", "EMAIL", "PASSWORD"));
        var childProfile = new ChildProfile(0, "name");

        profile.Childs.Add(childProfile);

        profile.Childs.Count.Should().Be(1);
    }

    public static IEnumerable<object[]> ProfileFavouriteData()
    {
        Profile profile = new Profile("ID", new User("LOGIN", "EMAIL", "PASSWORD"));
        var childProfile = new ChildProfile(0, "name");

        yield return new object[] { profile.Scored };
        yield return new object[] { profile.Watched };
        yield return new object[] { profile.WillWatch };
    }

}

