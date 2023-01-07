using Appservices.CreateChildProfileDtos;
using Appservices.CreateProfileDtos;
using Appservices.OutputDtos;
using Appservices.Exceptions;
namespace Appservices;

public class ProfileInteractor
{
    ProfileRepository _repo;
    public ProfileInteractor(ProfileRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProfileDto> Create(CreateProfileDto profileInfoDto, CancellationToken token = default)
    {
        if (await _repo.CountBy(profileInfoDto.Email, profileInfoDto.Login, token) > 0)
            throw new UserAlreadyExistsException();


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
        var res = await _repo.FetchProfile(profileId);

        return res;
    }


    public async Task<bool> AddWatchded(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _repo.AddWatched(profileId, filmId, token);

        return res;
    }
    public async Task<bool> DeleteWatchded(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _repo.DeleteWatched(profileId, filmId, token);

        return res;
    }
    public async Task<bool> AddWillWatch(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _repo.AddWillWatch(profileId, filmId, token);

        return res;
    }
    public async Task<bool> DeleteWillWatch(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _repo.DeleteWillWatch(profileId, filmId, token);

        return res;
    }
    public async Task<bool> AddScored(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _repo.AddScored(profileId, filmId, token);

        return res;
    }
    public async Task<bool> DeleteScored(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _repo.DeleteScored(profileId, filmId, token);

        return res;
    }
}