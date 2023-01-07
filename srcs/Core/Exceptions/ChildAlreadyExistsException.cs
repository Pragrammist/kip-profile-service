using System.Reflection.Metadata;
namespace ProfileService.Core;

public class ChildAlreadyExistsException : Exception
{
    public ChildAlreadyExistsException(ChildProfile value) : base($"{value.Name} already in favourite")
    {

    }
}
