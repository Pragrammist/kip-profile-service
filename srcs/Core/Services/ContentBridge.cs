namespace Core;

public interface ContentBridge
{
    Task<bool> AddWatched(string filmId, CancellationToken token);

    Task<bool> DeleteWatched(string filmId, CancellationToken token);


    Task<bool> AddWillWatch(string filmId, CancellationToken token);

    Task<bool> DeleteWillWatch(string filmId, CancellationToken token);
    

    Task<bool> AddNotInteresting(string filmId, CancellationToken token);

    Task<bool> DeleteNotInteresting(string filmId, CancellationToken token);
    

    Task<bool> AddScore(string filmId, uint score, CancellationToken token);

}
