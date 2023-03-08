using Core.CreateProfileDtos;
using Core.CreateChildProfileDtos;
using Core.OutputDtos;


namespace Core;


public interface ProfileRepository
{

    Task<ProfileDto?> GetProfile(string profileId, CancellationToken token = default);

    Task<bool> AddChildProfile(CreateChildProfileDto profileInfo, CancellationToken token = default);

    Task<bool> RemoveChildProfile(string profileId, string name, CancellationToken token = default);

    Task<ProfileDto> CreateProfile(CreateProfileDto profileData, CancellationToken token = default);

    Task<ProfileDto> FetchProfile(string id, CancellationToken token = default);

    Task<long> CountBy(string? email = null, string? login = null, CancellationToken token = default);

    Task<bool> AddWatched(string profileId, string filmId, CancellationToken token = default);

    Task<bool> DeleteWatched(string profileId, string filmId, CancellationToken token = default);

    Task<bool> AddWillWatch(string profileId, string filmId, CancellationToken token = default);

    Task<bool> DeleteWillWatch(string profileId, string filmId, CancellationToken token = default);

    Task<bool> AddNotInteresting(string profileId, string filmId, CancellationToken token = default);

    Task<bool> DeleteNotInteresting(string profileId, string filmId, CancellationToken token = default);

    Task<bool> AddScored(string profileId, string filmId, CancellationToken token = default);

    Task<bool> DeleteScored(string profileId, string filmId, CancellationToken token = default);
}