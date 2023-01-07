namespace ProfileService.Core;

public sealed class WillWatch : FavouriteBase, IList<string>, IEnumerable<string>
{
    public WillWatch() { }
    public WillWatch(IList<string> films) : base(films)
    {
    }
}
