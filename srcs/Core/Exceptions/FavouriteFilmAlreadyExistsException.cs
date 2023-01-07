namespace ProfileService.Core;

public class FavouriteFilmAlreadyExistsException : Exception
{
    public FavouriteFilmAlreadyExistsException(object value) : base($"{value} already in favourite")
    {

    }
}
