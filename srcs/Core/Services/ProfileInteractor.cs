using Core.CreateChildProfileDtos;
using Core.CreateProfileDtos;
using Core.OutputDtos;
using Core.Exceptions;
namespace Core;

public class ProfileInteractor
{
    readonly ProfileRepository _repo;
    readonly PasswordHasher _hasher;
    readonly ResetPasswordCodeStore _passwordCodeStore;
    readonly EmailSender _emailSender;
    public ProfileInteractor(ProfileRepository repo, PasswordHasher hasher, ResetPasswordCodeStore passwordCodeStore, EmailSender emailSender)
    {
        _repo = repo;
        _hasher = hasher;
        _emailSender = emailSender;
        _passwordCodeStore = passwordCodeStore;
    }

    public async Task<ProfileDto> Create(CreateProfileDto profileInfoDto, CancellationToken token = default)
    {
        if (await _repo.CountBy(profileInfoDto.Email, profileInfoDto.Login, token) > 0)
            throw new UserAlreadyExistsException();

        var profileWithHashedPassword = await GetProfileWithHashedPassword(profileInfoDto);
        var res = await _repo.CreateProfile(profileWithHashedPassword, token);
        return res;
    }
    async Task<CreateProfileDto> GetProfileWithHashedPassword(CreateProfileDto profileInfoDto) => new (){
            Email = profileInfoDto.Email,
            Login = profileInfoDto.Login,
            Password = await _hasher.Hash(profileInfoDto.Password)
        };

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

    public async Task<ProfileDto> GetProfile(string loginOrEmail, string passaword, CancellationToken token = default)
    {
        var hashedPassword = await _hasher.Hash(passaword);
        var res = await _repo.FetchProfile(loginOrEmail, hashedPassword, token);
        return res;
    }


    public async Task<bool> ChangeEamil(string loginOrEmail, string passaword, string newEmail,CancellationToken token = default)
    {
        var hashedPassword = await _hasher.Hash(passaword);
        
        var profile = await _repo.FetchProfile(loginOrEmail, hashedPassword, token);

        if(profile is null)
            return false;

        var isChanged = await _repo.ChangeEmail(profile.Id, newEmail, token);

        return isChanged;
    }

    public async Task<bool> ChangePassword(string loginOrEmail, string passaword, string newPassword,CancellationToken token = default)
    {
        var hashedPassword = await _hasher.Hash(passaword);
        
        var profile = await _repo.FetchProfile(loginOrEmail, hashedPassword, token);
        
        if(profile is null)
            return false;

        var isChanged = await _repo.ChangePassword(profile.Id, newPassword, token);

        return isChanged;
    }

    public async Task<string?> SendCodeToEmail(string email, CancellationToken token = default)
    {
        var profile = await _repo.FindByLoginOrEmail(email, token);
        if(profile is null)
            return null;
        var code = await _passwordCodeStore.GenerateCode(profile.Id, token);

        await _emailSender.SendCode(code, email, token);

        return code;
    }

    public async Task<bool> ResetPassword(string email, string code, string newPassaword, CancellationToken token = default)
    {
        var profile = await _repo.FindByLoginOrEmail(email, token);
        if(profile is null)
            return false;
        if(!await _passwordCodeStore.CodeIsValid(profile.Id, code, token))
            return false;

        
        var hashedPassword = await _hasher.Hash(newPassaword);
        return await _repo.SetPassword(profile.Id, hashedPassword, token);
    }
}