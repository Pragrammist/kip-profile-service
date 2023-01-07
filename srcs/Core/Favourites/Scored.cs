namespace ProfileService.Core;

public class Scored : FavouriteBase, IList<string>, IEnumerable<string>
{
    public Scored() { }
    public Scored(IList<string> films) : base(films)
    {
    }
}
