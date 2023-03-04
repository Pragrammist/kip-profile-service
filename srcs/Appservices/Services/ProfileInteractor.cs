using Appservices.CreateChildProfileDtos;
using Appservices.CreateProfileDtos;
using Appservices.OutputDtos;
using Appservices.Exceptions;
namespace Appservices;

public class ProfileInteractor
{
    readonly ProfileRepository _repo;
    readonly PasswordHasher _hasher;
    public ProfileInteractor(ProfileRepository repo, PasswordHasher hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    public async Task<ProfileDto> Create(CreateProfileDto profileInfoDto, CancellationToken token = default)
    {
        if (await _repo.CountBy(profileInfoDto.Email, profileInfoDto.Login, token) > 0)
            throw new UserAlreadyExistsException();

        profileInfoDto.Password = await _hasher.Hash(profileInfoDto.Password);

        var res = await _repo.CreateProfile(profileInfoDto, token);
        return res;
    }


    public async Task<bool> AddChildProfile(CreateChildProfileDto childInfoDto, CancellationToken token = default)
    {
        var res = await _repo.AddChildProfile(childInfoDto, token);

        return res;
    }
    public async Task<bool> RemoveChildProfile(string profileId, string name, CancellationToken token = default)
    {
        var res = await _repo.RemoveChildProfile(profileId, name, token);

        return res;
    }
    public async Task<ProfileDto> GetProfile(string profileId, CancellationToken token = default)
    {
        var res = await _repo.FetchProfile(profileId, token);

        return res;
    }
}