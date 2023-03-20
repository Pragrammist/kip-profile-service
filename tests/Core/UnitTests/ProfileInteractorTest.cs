using System.Collections.Generic;
using System.Threading;
using Xunit;
using Moq;
using FluentAssertions;
using Core;
using System.Threading.Tasks;
using Core.CreateProfileDtos;
using Core.OutputDtos;
using Core.Exceptions;

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
    ResetPasswordCodeStore GetMockResetPasswordCodeStore()
    {
        var mock = new Mock<ResetPasswordCodeStore>();

        return mock.Object;
    }
    EmailSender GetMockEmailSender()
    {
        var mock = new Mock<EmailSender>();

        return mock.Object;
    }
    ProfileRepository GetProfileRepoForFavourites()
    {
        var repo = new Mock<ProfileRepository>();
        repo.Setup(repo => repo.GetProfile(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ProfileDto{
            Id = "someProfileId",
            Watched = new List<string> {"filmid"},
            WhillWatch = new List<string> {"filmid"},
            Scored = new List<string> {"filmid"},
            NotInteresting = new List<string> {"filmid"}
        });
        return repo.Object;
    }
    void SetCreatepProfileMethod(Mock<ProfileRepository> repo)
    {
        repo.Setup(repo => repo.CreateProfile(It.IsAny<CreateProfileDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ProfileDto { Id = "someProfileId" });
    }
    ProfileInteractor ProfileInteractor(ProfileRepository repo, PasswordHasher hasher, ResetPasswordCodeStore codeStore, EmailSender sender) => new(repo, hasher, codeStore, sender);

    PasswordHasher PasswordHasher()
    {
        var hasher = new Mock<PasswordHasher>();
        hasher.Setup(t => t.Hash(It.IsAny<string>())).ReturnsAsync((string pass) => pass);
        return hasher.Object;
    }

    ProfileFavouritesInteractor ProfileFavInteractor(ProfileRepository repo, ContentBridge bridge) => new(repo, bridge);

    [Fact]
    public async Task CreateExistingUser()
    {
        var interactor = ProfileInteractor(GetProfileRepoForCreateProfileForExistingUser(), PasswordHasher(), GetMockResetPasswordCodeStore(), GetMockEmailSender());


        await Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await interactor.Create(new CreateProfileDto { Login = loginThatExists }));
    }

    [Fact]
    public async Task CreateNotExistingUser()
    {
        var interactor = ProfileInteractor(GetProfileRepoForCreateProfileForNotExistingUser(), PasswordHasher(), GetMockResetPasswordCodeStore(), GetMockEmailSender());
        var res = await interactor.Create(new CreateProfileDto { Email = emailThatDoesntExists });
        res.Id.Should().NotBeNull();
    }

    [Fact]
    public async Task AddExistingWatchedFilm()
    {
        var interactor = ProfileFavInteractor(GetProfileRepoForFavourites(), GetMockContentBridge());

        var res = await interactor.AddWatched("profileid", "filmid");

        res.Should().BeFalse();
    }


    [Fact]
    public async Task AddExistingScoredFilm()
    {
        var interactor = ProfileFavInteractor(GetProfileRepoForFavourites(), GetMockContentBridge());

        var res = await interactor.AddScored("profileid", "filmid", 5);

        res.Should().BeFalse();
    }


    [Fact]
    public async Task AddExistingWillWatchFilm()
    {
        var interactor = ProfileFavInteractor(GetProfileRepoForFavourites(), GetMockContentBridge());

        var res = await interactor.AddScored("profileid", "filmid", 5);

        res.Should().BeFalse();
    }

    [Fact]
    public async Task AddExistingNotIterestingFilm()
    {
        var interactor = ProfileFavInteractor(GetProfileRepoForFavourites(), GetMockContentBridge());

        var res = await interactor.AddNotInteresting("profileid", "filmid");

        res.Should().BeFalse();
    }
}