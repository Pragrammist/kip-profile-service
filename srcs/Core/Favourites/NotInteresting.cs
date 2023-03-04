namespace ProfileService.Core;

public sealed class NotInteresting : FavouriteBase, IList<string>, IEnumerable<string>
{
    public NotInteresting() { }
    public NotInteresting(IList<string> films) : base(films)
    {
    }
}
