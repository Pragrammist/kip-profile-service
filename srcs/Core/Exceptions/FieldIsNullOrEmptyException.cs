namespace ProfileService.Core;

public class FieldIsNullOrEmptyException : Exception
{
    public FieldIsNullOrEmptyException(string fieldName, string objName) : base($"field {fieldName} inner {objName} has null")
    {

    }
}