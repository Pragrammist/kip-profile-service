namespace ProfileService.Core;

public class ChildIndexOutOfRangeException : Exception
{
    public ChildIndexOutOfRangeException(int index, int count) : base($"{index} >= {count}")
    {

    }
}
