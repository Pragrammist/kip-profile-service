using MongoDB.Driver;
using ProfileService.Core;
using Appservices.OutputDtos;
using Appservices.CreateChildProfileDtos;
using Appservices.CreateProfileDtos;
using Mapster;
using Appservices;

namespace Infrastructure;

public class ProfileRepositoryImpl : ProfileRepository
{
    readonly IMongoCollection<Profile> _profileRepo;
    public ProfileRepositoryImpl(IMongoCollection<Profile> profileRepo)
    {
        _profileRepo = profileRepo;
    }


    UpdateDefinition<Profile> AddChild(ChildProfile profile) => Builders<Profile>.Update.AddToSet(p => p.Childs, profile);

    UpdateDefinition<Profile> DeleteChild(string name) => Builders<Profile>.Update.PullFilter(p => p.Childs, p => p.Name == name);


    UpdateDefinition<Profile> AddWatched(string filmId) => Builders<Profile>.Update.AddToSet(p => p.Watched, filmId);

    UpdateDefinition<Profile> DeleteWatched(string filmId) => Builders<Profile>.Update.Pull(p => p.Watched, filmId);


    UpdateDefinition<Profile> AddWillWatch(string filmId) => Builders<Profile>.Update.AddToSet(p => p.WillWatch, filmId);

    UpdateDefinition<Profile> DeleteWillWatch(string filmId) => Builders<Profile>.Update.Pull(p => p.WillWatch, filmId);


    UpdateDefinition<Profile> AddScored(string filmId) => Builders<Profile>.Update.AddToSet(p => p.Scored, filmId);

    UpdateDefinition<Profile> DeleteScored(string filmId) => Builders<Profile>.Update.Pull(p => p.Scored, filmId);

    public async Task<bool> AddChildProfile(CreateChildProfileDto profileInfo, CancellationToken token = default)
    {
        var childProfile = profileInfo.Adapt<ChildProfile>();

        var updateResult = await _profileRepo.UpdateOneAsync(f => f.Id == profileInfo.ProfileId, AddChild(childProfile), cancellationToken: token);

        return updateResult.ModifiedCount > 0;

    }

    public async Task<bool> RemoveChildProfile(string profileId, string name, CancellationToken token = default)
    {
        var updateResult = await _profileRepo.UpdateOneAsync(t => t.Id == profileId, DeleteChild(name), cancellationToken: token);
        return updateResult.ModifiedCount > 0;
    }

    public async Task<ProfileDto> CreateProfile(CreateProfileDto profileData, CancellationToken token = default)
    {
        var profile = profileData.Adapt<Profile>();

        await _profileRepo.InsertOneAsync(profile, cancellationToken: token);

        return profile.Adapt<ProfileDto>();
    }

    public async Task<ProfileDto> FetchProfile(string id, CancellationToken token = default)
    {
        var findResult = await _profileRepo.FindAsync(t => t.Id == id, cancellationToken: token);

        var profile = await findResult.FirstOrDefaultAsync();

        return profile.Adapt<ProfileDto>();
    }

    public async Task<long> CountBy(string? email = null, string? login = null, CancellationToken token = default)
    {
        if (email is not null && login is not null)
            return await _profileRepo.CountDocumentsAsync(f => (f.User.Email == email || f.User.Login == login), cancellationToken: token);
        if (email is not null)
            return await _profileRepo.CountDocumentsAsync(f => (f.User.Email == email), cancellationToken: token);
        if (login is not null)
            return await _profileRepo.CountDocumentsAsync(f => (f.User.Login == login), cancellationToken: token);
        else
            return await _profileRepo.CountDocumentsAsync(t => true, cancellationToken: token);
    }

    public async Task<bool> AddWatched(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _profileRepo.UpdateOneAsync(p => p.Id == profileId, AddWatched(filmId), cancellationToken: token);

        return res.ModifiedCount > 0;
    }

    public async Task<bool> DeleteWatched(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _profileRepo.UpdateOneAsync(p => p.Id == profileId, DeleteWatched(filmId), cancellationToken: token);

        return res.ModifiedCount > 0;
    }

    public async Task<bool> AddWillWatch(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _profileRepo.UpdateOneAsync(p => p.Id == profileId, AddWillWatch(filmId), cancellationToken: token);

        return res.ModifiedCount > 0;
    }

    public async Task<bool> DeleteWillWatch(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _profileRepo.UpdateOneAsync(p => p.Id == profileId, DeleteWillWatch(filmId), cancellationToken: token);

        return res.ModifiedCount > 0;
    }

    public async Task<bool> AddScored(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _profileRepo.UpdateOneAsync(p => p.Id == profileId, AddScored(filmId), cancellationToken: token);

        return res.ModifiedCount > 0;
    }

    public async Task<bool> DeleteScored(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _profileRepo.UpdateOneAsync(p => p.Id == profileId, DeleteScored(filmId), cancellationToken: token);

        return res.ModifiedCount > 0;
    }
}