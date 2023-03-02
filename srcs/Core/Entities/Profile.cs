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


    public IList<string> WillWatch { get => _willWatch; set {
            if(value.Any(s => string.IsNullOrWhiteSpace(s)))
                throw new NullOrEmptyInnerCollectionException(nameof(WillWatch), nameof(Profile));

            _willWatch = new WillWatch(value);
        } 
    }


    public IList<string> Scored { get => _scored; set {  
            if(value.Any(s => string.IsNullOrWhiteSpace(s)))
                throw new NullOrEmptyInnerCollectionException(nameof(Scored), nameof(Profile));

            _scored = new Scored(value); 
        } 
    }


    public IList<string> Watched { get => _watched; set { 
            if(value.Any(s => string.IsNullOrWhiteSpace(s)))
                    throw new NullOrEmptyInnerCollectionException(nameof(WillWatch), nameof(Profile));
                    
            _watched = new Watched(value); 
        } 
    }
}
