namespace Appservices.CreateChildProfileDtos;

public class CreateChildProfileDto
{
    public string ProfileId { get; set; } = null!;
    public int Age { get; set; }
    public string Name { get; set; } = null!;
    public int Gender { get; set; }
}