using Xunit;
using ProfileService.Core;
using FluentAssertions;
using System.Collections.Generic;
using System.Collections;

namespace UnitTests;

public class ChildProfileUnitTest
{
    [Theory]
    [ClassData(typeof(AgeLogicData))]
    [Trait("Category", "ChildProfile")]
    public void AgeLogic(int age, int expected)
    {
        ChildProfile profile = new ChildProfile(age, "name");

        profile.Age.Should().Be(expected);
    }

    [Fact]
    public void GenderDefaultValue()
    {
        ChildProfile profile = new ChildProfile(0, "name");

        profile.Gender.Should().Be(Gender.MALE);
    }
}


public class AgeLogicData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { -1, 0 };
        yield return new object[] { 0, 0 };
        yield return new object[] { 3, 0 };
        yield return new object[] { 5, 0 };
        yield return new object[] { 6, 6 };
        yield return new object[] { 7, 6 };
        yield return new object[] { 10, 6 };
        yield return new object[] { 11, 6 };
        yield return new object[] { 12, 12 };
        yield return new object[] { 13, 12 };
        yield return new object[] { 14, 12 };
        yield return new object[] { 15, 12 };
        yield return new object[] { 16, 16 };
        yield return new object[] { 17, 16 };
        yield return new object[] { 128, 16 };
        yield return new object[] { 127, 16 };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}