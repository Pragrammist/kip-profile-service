namespace ProfileService.Core;

public class Watched : FavouriteBase, IList<string>, IEnumerable<string>
{
    public Watched() { }
    public Watched(IList<string> films) : base(films)
    {
    }
}
