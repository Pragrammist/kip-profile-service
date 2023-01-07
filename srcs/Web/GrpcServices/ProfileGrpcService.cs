using Grpc.Core;
using GrpcProfileService;
using Appservices;
using Mapster;
using Appservices.CreateProfileDtos;
using Web.Services;

namespace Web;

public class ProfileGrpcService : Profile.ProfileBase
{
    readonly ProfileInteractor _profile;
    public ProfileGrpcService(ProfileInteractor profile)
    {
        _profile = profile;
    }
    public override async Task<ProfileResponse> CreateProfile(CreateProfileRequest request, ServerCallContext context)
    {
        var profileData = request.Adapt<CreateProfileDto>();
        var profile = await _profile.Create(profileData);

        return profile.Adapt<ProfileResponse>();
    }
}
