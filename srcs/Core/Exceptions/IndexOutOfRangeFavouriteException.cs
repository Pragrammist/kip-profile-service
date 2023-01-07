namespace ProfileService.Core;

public class IndexOutOfRangeFavouriteException : Exception
{
    public IndexOutOfRangeFavouriteException(int index, int count) : base($"{index} >= {count}")
    {

    }
}
