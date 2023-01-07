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

    void SetCreatepProfileMethod(Mock<ProfileRepository> repo)
    {
        repo.Setup(repo => repo.CreateProfile(It.IsAny<CreateProfileDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ProfileDto { Id = "someProfileId" });
    }
    ProfileInteractor ProfileInteractor(ProfileRepository repo) => new ProfileInteractor(repo);


    [Fact]
    public async Task CreateExistingUser()
    {
        var interactor = ProfileInteractor(GetProfileRepoForCreateProfileForExistingUser());


        await Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await interactor.Create(new CreateProfileDto { Login = loginThatExists }));
    }
    [Fact]
    public async Task CreateNotExistingUser()
    {
        var interactor = ProfileInteractor(GetProfileRepoForCreateProfileForNotExistingUser());
        var res = await interactor.Create(new CreateProfileDto { Email = emailThatDoesntExists });
        res.Id.Should().NotBeNull();
    }
}