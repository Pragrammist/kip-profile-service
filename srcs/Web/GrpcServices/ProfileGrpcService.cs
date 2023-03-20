using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;
using Grpc.Core;
using GrpcProfileService;
using Core;
using Mapster;
using Core.CreateProfileDtos;
using Web.Services;
using System.Threading.Tasks;

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


    public override async Task<IsChangedResult> ChangeEmail(ChangeEmailRequest request, Grpc.Core.ServerCallContext context)
    {
        var res = await _profile.ChangeEamil(request.LoginOrEmail, request.Password, request.NewEmail, context.CancellationToken);
        
        return new IsChangedResult {
            IsChagned = res
        };
    }

    public async override Task<IsChangedResult> ChangePassword(ChangePasswordRequest request, Grpc.Core.ServerCallContext context)
    {
        var res = await _profile.ChangePassword(request.LoginOrEmail, request.Password, request.NewPassword, context.CancellationToken);

        return new IsChangedResult {
            IsChagned = res
        };
    }

    public override async Task<ProfileResponse> Login(LoginRequest request, Grpc.Core.ServerCallContext context)
    {
        var profile = await _profile.GetProfile(request.LoginOrEmail, request.Password, context.CancellationToken);
        return profile.Adapt<ProfileResponse>();
    }
    public override async Task<SendCodeToResetPasswordResponse> SendCodeToEmailToResetPassword(SendCodeToResetPasswordRequest request, ServerCallContext context)
    {
        var code = await _profile.SendCodeToEmail(request.Email, context.CancellationToken);
        var res = new SendCodeToResetPasswordResponse
        {
            Code = code
        };
        return res;
    }
    public override async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request, ServerCallContext context)
    {
        var isReseted = await _profile.ResetPassword(request.Email, request.Code, request.NewPassword, context.CancellationToken);
        return new ResetPasswordResponse
        {
            IsReseted = isReseted
        };
    }
}
