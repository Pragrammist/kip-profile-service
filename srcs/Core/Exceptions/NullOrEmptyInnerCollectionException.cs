namespace ProfileService.Core;

public class NullOrEmptyInnerCollectionException : Exception
{
    public NullOrEmptyInnerCollectionException(string collName, string objName) : base($"collection {collName} inner {objName} has null inner collection")
    {

    }
}
