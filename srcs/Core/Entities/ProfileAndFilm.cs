namespace ProfileService.Core;

public record class ProfileAndFilm
{
    public ProfileAndFilm(string filmId, string profileId)
    {
        FilmID = filmId;
        ProfileId = profileId;
    }

    public string FilmID { get; init; } = null!;

    public string ProfileId { get; init; } = null!;

}
