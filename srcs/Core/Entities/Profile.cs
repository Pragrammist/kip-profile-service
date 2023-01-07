namespace ProfileService.Core;

public class Profile
{
    IList<string> _willWatch = new WillWatch();
    IList<string> _scored = new Scored();
    IList<string> _watched = new Watched();
    IList<ChildProfile> _childs = new Childern();


    public Profile(User user)
    {
        User = user;
    }
    public Profile(string id, User user)
    {
        Id = id;
        User = user;
    }
    private Profile() { }

    public string Id { get; set; } = null!;


    public User User { get; set; } = null!;


    public IList<ChildProfile> Childs { get => _childs; set => _childs = new Childern(value); }


    public IList<string> WillWatch { get => _willWatch; set => _willWatch = new WillWatch(value); }


    public IList<string> Scored { get => _scored; set => _scored = new Scored(value); }


    public IList<string> Watched { get => _watched; set => _watched = new Watched(value); }


}
