using System.Collections.Generic;
using System.Data;
using System.Collections;


namespace ProfileService.Core;


public abstract class FavouriteBase : IList<string>, IEnumerable<string>
{
    protected readonly IList<string> _films = null!;



    protected readonly string _profileId = null!;
    protected FavouriteBase()
    {
        _films = new List<string>();
    }
    protected FavouriteBase(IList<string> films)
    {
        _films = films;
    }
    public string this[int index] { get => _films[index]; set => Insert(index, value); }

    public int Count => _films.Count;

    public bool IsReadOnly => false;


    private void AddFilmValidation(string filmId)
    {
        if (_films.FirstOrDefault(t => t == filmId) != null)
            throw new FavouriteFilmAlreadyExistsException(filmId);
    }

    public void Add(string filmId)
    {
        AddFilmValidation(filmId);
        _films.Add(filmId);
    }

    public void Clear() => _films.Clear();


    public bool Contains(string filmId) => _films.FirstOrDefault(t => t == filmId) is not null;


    public void CopyTo(string[] array, int arrayIndex) => _films.CopyTo(array, arrayIndex);





    public IEnumerator<string> GetEnumerator() => _films.GetEnumerator();


    public int IndexOf(string filmId) => _films.IndexOf(filmId);


    public void Insert(int index, string filmId)
    {
        AddFilmValidation(filmId);
        if (index >= Count)
            throw new IndexOutOfRangeFavouriteException(index, Count);

        _films[index] = filmId;
    }


    public bool Remove(string filmId) => _films.Remove(filmId);


    public void RemoveAt(int index) => _films.RemoveAt(index);


    IEnumerator IEnumerable.GetEnumerator() => _films.GetEnumerator();
}