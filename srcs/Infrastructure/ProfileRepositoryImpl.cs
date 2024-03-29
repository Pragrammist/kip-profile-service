using MongoDB.Driver;
using ProfileService.Core;
using Core.OutputDtos;
using Core.CreateChildProfileDtos;
using Core.CreateProfileDtos;
using Mapster;
using Core;

namespace Infrastructure;

public class ProfileRepositoryImpl : ProfileRepository
{
    readonly IMongoCollection<Profile> _profileRepo;
    public ProfileRepositoryImpl(IMongoCollection<Profile> profileRepo)
    {
        _profileRepo = profileRepo;
    }


    static UpdateDefinition<Profile> AddChild(ChildProfile profile) => Builders<Profile>.Update.AddToSet(p => p.Childs, profile);

    static UpdateDefinition<Profile> DeleteChild(string name) => Builders<Profile>.Update.PullFilter(p => p.Childs, p => p.Name == name);


    static UpdateDefinition<Profile> AddWatched(string filmId) => Builders<Profile>.Update.AddToSet(p => p.Watched, filmId);

    static UpdateDefinition<Profile> DeleteWatched(string filmId) => Builders<Profile>.Update.Pull(p => p.Watched, filmId);


    static UpdateDefinition<Profile> AddWillWatch(string filmId) => Builders<Profile>.Update.AddToSet(p => p.WillWatch, filmId);

    static UpdateDefinition<Profile> DeleteWillWatch(string filmId) => Builders<Profile>.Update.Pull(p => p.WillWatch, filmId);


    static UpdateDefinition<Profile> AddScored(string filmId) => Builders<Profile>.Update.AddToSet(p => p.Scored, filmId);

    static UpdateDefinition<Profile> DeleteScored(string filmId) => Builders<Profile>.Update.Pull(p => p.Scored, filmId);


    static UpdateDefinition<Profile> AddNotInteresting(string filmId) => Builders<Profile>.Update.AddToSet(p => p.NotInteresting, filmId);

    static UpdateDefinition<Profile> DeleteNotInteresting(string filmId) => Builders<Profile>.Update.Pull(p => p.NotInteresting, filmId);



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

        var profile = await findResult.FirstOrDefaultAsync(token);

        return profile.Adapt<ProfileDto>();
    }

    public async Task<long> CountBy(string? email = null, string? login = null, CancellationToken token = default)
    {
        if (email is not null && login is not null)
            return await _profileRepo.CountDocumentsAsync(f => f.User.Email == email || f.User.Login == login, cancellationToken: token);
        if (email is not null)
            return await _profileRepo.CountDocumentsAsync(f => f.User.Email == email, cancellationToken: token);
        if (login is not null)
            return await _profileRepo.CountDocumentsAsync(f => f.User.Login == login, cancellationToken: token);
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

    public async Task<ProfileDto?> GetProfile(string profileId, CancellationToken token = default) => 
    (
        await 
        (
            await _profileRepo.FindAsync(p => p.Id == profileId, cancellationToken: token)
        )
        .FirstOrDefaultAsync(cancellationToken: token)
    )
    .Adapt<ProfileDto>();

    public async Task<bool> AddNotInteresting(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _profileRepo.UpdateOneAsync(p => p.Id == profileId, AddNotInteresting(filmId), cancellationToken: token);

        return res.ModifiedCount > 0;
    }

    public async Task<bool> DeleteNotInteresting(string profileId, string filmId, CancellationToken token = default)
    {
        var res = await _profileRepo.UpdateOneAsync(p => p.Id == profileId, DeleteNotInteresting(filmId), cancellationToken: token);

        return res.ModifiedCount > 0;
    }

    public async Task<ProfileDto> FetchProfile(string loginOrEmail, string hashedPassword, CancellationToken token = default)=> 
    (
        await 
        (
            await _profileRepo.FindAsync(p => (p.User.Login == loginOrEmail || p.User.Email == loginOrEmail) && p.User.Password == hashedPassword, cancellationToken: token)
        )
        .FirstOrDefaultAsync(cancellationToken: token)
    )
    .Adapt<ProfileDto>();

    public async  Task<bool> ChangeEmail(string id, string newEmail, CancellationToken token = default)
    => (await _profileRepo.UpdateOneAsync(
        filter:p => p.Id == id,
        update: Builders<Profile>.Update.Set(f => f.User.Email, newEmail),
        cancellationToken: token
    )).ModifiedCount > 0;

    public async Task<bool> ChangePassword(string id, string newPassword, CancellationToken token = default)
    => (await _profileRepo.UpdateOneAsync(
        filter:p => p.Id == id,
        update: Builders<Profile>.Update.Set(f => f.User.Password, newPassword),
        cancellationToken: token
    )).ModifiedCount > 0;

    public async Task<ProfileDto?> FindByLoginOrEmail(string loginOrEmail, CancellationToken token = default)
    {
        var findResult = await _profileRepo.FindAsync(t => t.User.Login == loginOrEmail || t.User.Email == loginOrEmail, cancellationToken: token);

        var profile = await findResult.FirstOrDefaultAsync(token);

        return profile.Adapt<ProfileDto>();
    }

    public async Task<bool> SetPassword(string id, string password, CancellationToken token = default)
    {
        var updateResult = await _profileRepo.UpdateOneAsync(
            u => u.Id == id, 
            Builders<Profile>.Update.Set(p => p.User.Password, password), 
            cancellationToken: token
        );
        return updateResult.ModifiedCount > 0;
    }
}