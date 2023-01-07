using System.Collections;
using System.Linq;

namespace ProfileService.Core;


public class Childern : IList<ChildProfile>
{
    IList<ChildProfile> _children = new List<ChildProfile>();



    public Childern(IList<ChildProfile> children)
    {
        _children = children;
    }

    public Childern()
    {
        _children = new List<ChildProfile>();
    }


    void ValidateChild(ChildProfile childProfile)
    {
        if (_children.FirstOrDefault(t => t.Name == childProfile.Name) is not null)
            throw new ChildAlreadyExistsException(childProfile);
    }

    ChildProfile IndexOrException(int index)
    {
        try
        {
            var res = _children[index];
            return res;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ChildIndexOutOfRangeException(index, Count);
        }

    }
    public ChildProfile this[int index] { get => IndexOrException(index); set => Insert(index, value); }

    public int Count => _children.Count;

    public bool IsReadOnly => false;

    public void Add(ChildProfile item)
    {
        ValidateChild(item);
        _children.Add(item);
    }

    public void Clear()
    {
        _children.Clear();
    }

    public bool Contains(ChildProfile item) => _children.FirstOrDefault(t => t.Name == item.Name) is not null;

    public void CopyTo(ChildProfile[] array, int arrayIndex) => throw new NotSupportedException("MICROSOFT FUCK YOU");


    public IEnumerator<ChildProfile> GetEnumerator() => _children.GetEnumerator();


    public int IndexOf(ChildProfile item)
    {
        for (int i = 0; i < _children.Count; i++)
        {
            if (_children[i].Name == item.Name)
                return i;
        }
        return -1;
    }

    public void Insert(int index, ChildProfile item)
    {
        IndexOrException(index);
        ValidateChild(item);
        _children[index] = item;
    }

    public bool Remove(ChildProfile item)
    {
        var el = _children.FirstOrDefault(t => t.Name == item.Name);
        if (el is null)
            return false;
        return _children.Remove(el);
    }

    public void RemoveAt(int index)
    {
        IndexOrException(index);
        _children.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();

}
