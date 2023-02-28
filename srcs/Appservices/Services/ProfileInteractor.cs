using Appservices.CreateChildProfileDtos;
using Appservices.CreateProfileDtos;
using Appservices.OutputDtos;
using Appservices.Exceptions;
namespace Appservices;

public class ProfileInteractor
{
    readonly ProfileRepository _repo;
    readonly ContentBridge _contentBridge;
    public ProfileInteractor(ProfileRepository repo, ContentBridge contentBridge)
    {
        _repo = repo;
        _contentBridge = contentBridge;
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
        var res = await _repo.FetchProfile(profileId, token);

        return res;
    }


    public async Task<bool> AddWatched(string profileId, string filmId, CancellationToken token = default)
    {
        var profile = await _repo.GetProfile(profileId, token);
        
        if(profile is null)
            return false;

        if(profile.Watched.Contains(filmId))
            return false;
            
        var isAdded = await _contentBridge.AddWatched(filmId, token);
        if(!isAdded)
            return false;
        return await _repo.AddWatched(profileId, filmId, token);
    }
    public async Task<bool> DeleteWatched(string profileId, string filmId, CancellationToken token = default)
    {
        var isAdded = await _contentBridge.DeleteWatched(filmId, token);
        if(!isAdded)
            return false;
        
        return await _repo.DeleteWatched(profileId, filmId, token);
    }
    public async Task<bool> AddWillWatch(string profileId, string filmId, CancellationToken token = default)
    {
        var profile = await _repo.GetProfile(profileId, token);
        
        if(profile is null)
            return false;

        if(profile.WhillWatch.Contains(filmId))
            return false;


        var isAdded = await _contentBridge.AddWillWatch(filmId, token);
        if(!isAdded)
            return false;
        return await _repo.AddWillWatch(profileId, filmId, token);
    }
    public async Task<bool> DeleteWillWatch(string profileId, string filmId, CancellationToken token = default)
    {
        var isAdded = await _contentBridge.DeleteWillWatch(filmId, token);
        if(!isAdded)
            return false;
        return await _repo.DeleteWillWatch(profileId, filmId, token);
    }
    public async Task<bool> AddScored(string profileId, string filmId, uint score, CancellationToken token = default)
    {
        var profile = await _repo.GetProfile(profileId, token);
        
        if(profile is null)
            return false;

        if(profile.Scored.Contains(filmId))
            return false;


        var isAdded = await _contentBridge.AddScore(filmId, score, token);
        if(!isAdded)
            return false;
        return await _repo.AddScored(profileId, filmId, token);
    }
    public async Task<bool> DeleteScored(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _repo.DeleteScored(profileId, filmId, token);

        return res;
    }
}