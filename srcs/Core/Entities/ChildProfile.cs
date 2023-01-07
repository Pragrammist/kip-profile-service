namespace ProfileService.Core;

public class ChildProfile
{
    private ChildProfile() { }
    public ChildProfile(int age, string name, Gender gender = default)
    {
        Age = age;
        Gender = gender;
        Name = name;
    }
    int _age;
    public int Age
    {
        get => _age; set
        {
            switch (value)
            {
                case < 6:
                    _age = 0;
                    break;
                case < 12:
                    _age = 6;
                    break;
                case < 16:
                    _age = 12;
                    break;
                case >= 16:
                    _age = 16;
                    break;
            }
        }
    }
    public Gender Gender { get; set; }
    public string Name { get; set; } = null!;
}
