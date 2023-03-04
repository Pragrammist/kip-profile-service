namespace Appservices;

public class ProfileFavouritesInteractor
{
    readonly ProfileRepository _repo;
    readonly ContentBridge _contentBridge;
    public ProfileFavouritesInteractor(ProfileRepository repo, ContentBridge contentBridge)
    {
        _repo = repo;
        _contentBridge = contentBridge;
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


    public async Task<bool> AddNotInteresting(string profileId, string filmId, CancellationToken token = default)
    {
        var profile = await _repo.GetProfile(profileId, token);
        
        if(profile is null)
            return false;

        if(profile.WhillWatch.Contains(filmId))
            return false;


        var isAdded = await _contentBridge.AddNotInteresting(filmId, token);
        if(!isAdded)
            return false;
        return await _repo.AddNotInteresting(profileId, filmId, token);
    }
    public async Task<bool> DeleteNotInteresting(string profileId, string filmId, CancellationToken token = default)
    {
        var isAdded = await _contentBridge.DeleteNotInteresting(filmId, token);
        if(!isAdded)
            return false;
        return await _repo.DeleteNotInteresting(profileId, filmId, token);
    }
}
