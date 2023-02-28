using System.Collections.Generic;
using System.Threading;
using Xunit;
using Moq;
using FluentAssertions;
using Appservices;
using System.Threading.Tasks;
using Appservices.CreateProfileDtos;
using Appservices.OutputDtos;
using Appservices.Exceptions;

namespace UnitTests;

public class ProfileInteractorTest
{
    readonly string loginThatExists = "i'm existing user";
    readonly string emailThatDoesntExists = "i'm not existing user";
    ProfileRepository GetProfileRepoForCreateProfileForExistingUser()
    {
        var repo = new Mock<ProfileRepository>();
        repo.Setup(t => t.CountBy(It.IsAny<string>(), loginThatExists, It.IsAny<CancellationToken>())).ReturnsAsync(1);
        SetCreatepProfileMethod(repo);
        return repo.Object;
    }

    ProfileRepository GetProfileRepoForCreateProfileForNotExistingUser()
    {
        var repo = new Mock<ProfileRepository>();
        repo.Setup(t => t.CountBy(emailThatDoesntExists, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        SetCreatepProfileMethod(repo);
        return repo.Object;
    }
    ContentBridge GetMockContentBridge()
    {
        var repo = new Mock<ContentBridge>();

        return repo.Object;
    }

    ProfileRepository GetProfileRepoForFavourites()
    {
        var repo = new Mock<ProfileRepository>();
        repo.Setup(repo => repo.GetProfile(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ProfileDto{
            Id = "someProfileId",
            Watched = new List<string> {"filmid"},
            WhillWatch = new List<string> {"filmid"},
            Scored = new List<string> {"filmid"}
        });
        return repo.Object;
    }
    void SetCreatepProfileMethod(Mock<ProfileRepository> repo)
    {
        repo.Setup(repo => repo.CreateProfile(It.IsAny<CreateProfileDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ProfileDto { Id = "someProfileId" });
    }
    ProfileInteractor ProfileInteractor(ProfileRepository repo, ContentBridge bridge) => new(repo, bridge);


    [Fact]
    public async Task CreateExistingUser()
    {
        var interactor = ProfileInteractor(GetProfileRepoForCreateProfileForExistingUser(), GetMockContentBridge());


        await Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await interactor.Create(new CreateProfileDto { Login = loginThatExists }));
    }

    [Fact]
    public async Task CreateNotExistingUser()
    {
        var interactor = ProfileInteractor(GetProfileRepoForCreateProfileForNotExistingUser(), GetMockContentBridge());
        var res = await interactor.Create(new CreateProfileDto { Email = emailThatDoesntExists });
        res.Id.Should().NotBeNull();
    }

    [Fact]
    public async Task AddExistingWatchedFilm()
    {
        var interactor = ProfileInteractor(GetProfileRepoForFavourites(), GetMockContentBridge());

        var res = await interactor.AddWatched("profileid", "filmid");

        res.Should().BeFalse();
    }


    [Fact]
    public async Task AddExistingScoredFilm()
    {
        var interactor = ProfileInteractor(GetProfileRepoForFavourites(), GetMockContentBridge());

        var res = await interactor.AddScored("profileid", "filmid", 5);

        res.Should().BeFalse();
    }


    [Fact]
    public async Task AddExistingWillWatchFilm()
    {
        var interactor = ProfileInteractor(GetProfileRepoForFavourites(), GetMockContentBridge());

        var res = await interactor.AddScored("profileid", "filmid", 5);

        res.Should().BeFalse();
    }
}