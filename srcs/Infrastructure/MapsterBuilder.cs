using ProfileService.Core;
using Mapster;
using Appservices.CreateProfileDtos;
using Appservices.CreateChildProfileDtos;
using Appservices.OutputDtos;

namespace Infrastructure;

public class MapsterBuilder
{

    public static void ConfigureMapster()
    {
        TypeAdapterConfig<CreateProfileDto, Profile>.NewConfig().ConstructUsing(p => new Profile(new User(p.Login, p.Email, p.Password)));
        TypeAdapterConfig<CreateChildProfileDto, ChildProfile>.NewConfig().ConstructUsing(ch => new ChildProfile(ch.Age, ch.Name, (Gender)ch.Gender));
        TypeAdapterConfig<Profile, ProfileDto>.NewConfig()
            .Map(dest => dest.WhillWatch, src => src.WillWatch.ToList())
            .Map(dest => dest.Watched, src => src.Watched.ToList())
            .Map(dest => dest.Scored, src => src.Scored.ToList());
    }
}
